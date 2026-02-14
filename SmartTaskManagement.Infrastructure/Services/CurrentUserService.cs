using Microsoft.AspNetCore.Http;
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

    public Guid UserId =>
        Guid.Parse(_httpContextAccessor.HttpContext!
            .User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public bool IsAdmin =>
        _httpContextAccessor.HttpContext!
            .User.IsInRole("Admin");
}