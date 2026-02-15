using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartTaskManagement.Application.DTOs.User;
using SmartTaskManagement.Application.Interfaces;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmartTaskManagement.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new Exception("Invalid credentials");

        var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
            throw new Exception("Invalid credentials");

        var accessToken = await GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            UserId = user.Id
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            throw new Exception("User already exists");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "User");

        var accessToken = await GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            UserId = user.Id
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (storedToken == null ||
            storedToken.IsRevoked ||
            storedToken.Expires < DateTime.UtcNow)
        {
            throw new Exception("Invalid refresh token");
        }

        storedToken.IsRevoked = true;

        var newRefreshToken = GenerateRefreshToken();

        var newTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshToken,
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            UserId = storedToken.UserId
        };

        _context.RefreshTokens.Add(newTokenEntity);

        var newAccessToken = await GenerateJwtToken(storedToken.User);

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var keyString = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(keyString))
            throw new Exception("JWT Key is missing in configuration!");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}