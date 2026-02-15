using Microsoft.AspNetCore.Http;
using SmartTaskManagement.Application.Exceptions;
using SmartTaskManagement.Application.Interfaces;
using System.Security.Claims;

namespace SmartTaskManagement.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            return userId != null
                ? Guid.Parse(userId)
                : throw new UnauthorizedException("User is not authenticated");
        }
    }

    public bool IsAdmin =>
        _httpContextAccessor.HttpContext?
            .User?
            .IsInRole("Admin") ?? false;
}