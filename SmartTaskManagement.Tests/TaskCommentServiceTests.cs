using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SmartTaskManagement.Application.DTOs.Comment;
using SmartTaskManagement.Application.Interfaces;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Infrastructure.Data;
using SmartTaskManagement.Infrastructure.Services;

namespace SmartTaskManagement.Tests;

public class TaskCommentServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly IMapper _mapper;
    private readonly TaskCommentService _service;

    private readonly Guid _userId = Guid.NewGuid();

    public TaskCommentServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(x => x.UserId).Returns(_userId);
        _currentUserMock.Setup(x => x.IsAdmin).Returns(false);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TaskComment, CreateCommentDto>();
        });

        _mapper = config.CreateMapper();

        _service = new TaskCommentService(_context, _currentUserMock.Object, _mapper);
    }

    [Fact]
    public async Task AddComment_Should_Add_Comment_To_Task()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            UserId = Guid.NewGuid()
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var commentId = await _service.CreateAsync(task.Id, new CreateCommentDto() { Content = "Test Comment" });

        // Assert
        var comment = await _context.TaskComments.FindAsync(commentId);

        comment.Should().NotBeNull();
        comment!.Content.Should().Be("Test Comment");
    }
}