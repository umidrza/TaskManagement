using SmartTaskManagement.Application.DTOs.User;

namespace SmartTaskManagement.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto request);
    Task<AuthResponseDto> LoginAsync(LoginDto request);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
}
