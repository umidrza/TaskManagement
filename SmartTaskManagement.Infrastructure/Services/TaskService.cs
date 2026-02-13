using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Task;
using SmartTaskManagement.Application.Interfaces;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAsync(Guid userId, CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            UserId = userId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return task.Id;
    }

    public async Task<PagedResult<TaskDto>> GetAllAsync(
        Guid userId,
        bool isAdmin,
        TaskItemStatus? status,
        int page,
        int pageSize)
    {
        var query = _context.Tasks.AsQueryable();

        if (!isAdmin)
            query = query.Where(t => t.UserId == userId);

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

    public async Task UpdateAsync(
        Guid userId,
        bool isAdmin,
        Guid taskId,
        UpdateTaskDto dto)
    {
        var task = await _context.Tasks.FindAsync(taskId);

        if (task == null)
            throw new Exception("Task not found");

        if (!isAdmin && task.UserId != userId)
            throw new UnauthorizedAccessException();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        Guid userId,
        bool isAdmin,
        Guid taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);

        if (task == null)
            throw new Exception("Task not found");

        if (!isAdmin && task.UserId != userId)
            throw new UnauthorizedAccessException();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }
}