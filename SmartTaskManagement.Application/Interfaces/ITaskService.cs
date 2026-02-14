using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Task;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<Guid> CreateAsync(CreateTaskDto dto);

    Task<PagedResult<TaskDto>> GetAllAsync(TaskItemStatus? status, int page, int pageSize);

    Task UpdateAsync(Guid taskId, UpdateTaskDto dto);

    Task DeleteAsync(Guid taskId);
}