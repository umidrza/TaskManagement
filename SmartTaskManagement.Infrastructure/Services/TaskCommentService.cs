using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.DTOs.Comment;
using SmartTaskManagement.Application.Interfaces;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Services;

public class TaskCommentService : ITaskCommentService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public TaskCommentService(
        ApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> CreateAsync(Guid taskId, CreateCommentDto dto)
    {
        var comment = new TaskComment
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = _currentUser.UserId,
            Content = dto.Content,
        };

        _context.TaskComments.Add(comment);
        await _context.SaveChangesAsync();

        return comment.Id;
    }

}
