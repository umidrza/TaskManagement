using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.Exceptions;
using SmartTaskManagement.Application.DTOs.Task;
using SmartTaskManagement.Application.Interfaces;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public TaskService(
        ApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> CreateAsync(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            UserId = _currentUser.UserId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return task.Id;
    }

    public async Task<PagedResult<TaskDto>> GetAllAsync(
        TaskItemStatus? status,
        int page,
        int pageSize)
    {
        var query = _context.Tasks.AsQueryable();

        if (!_currentUser.IsAdmin)
            query = query.Where(t => t.UserId == _currentUser.UserId);

        if (status.HasValue)
            query = query.Where(t => t.Status == status);

        var totalCount = await query.CountAsync();

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<TaskDto>
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Data = tasks
        };
    }

    public async Task UpdateAsync(Guid taskId, UpdateTaskDto dto)
    {
        var task = await _context.Tasks.FindAsync(taskId);

        if (task == null)
            throw new NotFoundException("Task not found");

        if (!_currentUser.IsAdmin && task.UserId != _currentUser.UserId)
            throw new ForbiddenException("You are not allowed to update this task.");

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.Priority = dto.Priority;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);

        if (task == null)
            throw new NotFoundException("Task not found");

        if (!_currentUser.IsAdmin && task.UserId != _currentUser.UserId)
            throw new ForbiddenException("You are not allowed to delete this task.");

        task.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task<List<TaskDto>> GetOverdueAsync()
    {
        var query = _context.Tasks
            .Where(t => t.IsOverdue);

        return await query
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                Priority = t.Priority,
                CreatedAt = t.CreatedAt,
            })
            .ToListAsync();
    }

    public async Task<TaskStatsDto> GetStatsAsync()
    {
        var query = _context.Tasks.AsQueryable();

        return new TaskStatsDto
        {
            Total = await query.CountAsync(),
            Completed = await query.CountAsync(t => t.Status == TaskItemStatus.Completed),
            Pending = await query.CountAsync(t => t.Status == TaskItemStatus.Pending),
            InProgress = await query.CountAsync(t => t.Status == TaskItemStatus.InProgress)
        };
    }
}
