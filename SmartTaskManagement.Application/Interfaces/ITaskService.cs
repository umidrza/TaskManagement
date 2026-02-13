using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Task;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<Guid> CreateAsync(Guid userId, CreateTaskDto dto);

    Task<PagedResult<TaskDto>> GetAllAsync(
        Guid userId,
        bool isAdmin,
        TaskItemStatus? status,
        int page,
        int pageSize);

    Task UpdateAsync(
        Guid userId,
        bool isAdmin,
        Guid taskId,
        UpdateTaskDto dto);

    Task DeleteAsync(
        Guid userId,
        bool isAdmin,
        Guid taskId);
}
