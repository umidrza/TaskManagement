using SmartTaskManagement.Application.DTOs.Comment;

namespace SmartTaskManagement.Application.Interfaces;

public interface ITaskCommentService
{
    Task<Guid> CreateAsync(Guid taskId, CreateCommentDto dto);
}
