using Microsoft.AspNetCore.Identity;

namespace SmartTaskManagement.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public ICollection<TaskItem> Tasks { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
