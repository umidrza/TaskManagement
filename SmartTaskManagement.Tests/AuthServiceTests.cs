using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SmartTaskManagement.Application.DTOs.User;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Infrastructure.Data;
using SmartTaskManagement.Infrastructure.Services;

namespace SmartTaskManagement.Tests;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthServiceTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!,
            null!, null!, null!, null!);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:Key", "SuperSecretKeySuperSecretKey12345" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }

    [Fact]
    public async Task Login_Should_Return_Tokens_When_Credentials_Are_Valid()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>()))
            .ReturnsAsync(true);

        _userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        var service = new AuthService(
            _userManagerMock.Object,
            _context,
            _configuration);

        // Act
        var result = await service.LoginAsync(new LoginDto
        {
            Email = "test@test.com",
            Password = "123456"
        });

        // Assert
        result.AccessToken.Should().NotBeNull();
        result.RefreshToken.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_Should_Throw_When_User_Not_Found()
    {
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        var service = new AuthService(
            _userManagerMock.Object,
            _context,
            _configuration);

        await Assert.ThrowsAsync<Exception>(() =>
            service.LoginAsync(new LoginDto
            {
                Email = "wrong@test.com",
                Password = "123"
            }));
    }
}