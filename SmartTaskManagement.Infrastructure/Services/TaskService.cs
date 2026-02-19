using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Task;
using SmartTaskManagement.Application.Exceptions;
using SmartTaskManagement.Application.Interfaces;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public TaskService(
        ApplicationDbContext context,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    private IQueryable<TaskItem> ApplyUserFilter(IQueryable<TaskItem> query)
    {
        if (!_currentUser.IsAdmin)
            query = query.Where(t => t.UserId == _currentUser.UserId);

        return query;
    }

    public async Task<Guid> CreateAsync(CreateTaskDto dto)
    {
        var task = _mapper.Map<TaskItem>(dto);

        task.Id = Guid.NewGuid();
        task.UserId = _currentUser.UserId;

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return task.Id;
    }

    public async Task<PagedResult<TaskDto>> GetAllAsync(
        TaskItemStatus? status,
        int page,
        int pageSize)
    {
        var query = ApplyUserFilter(_context.Tasks.AsNoTracking());

        if (status.HasValue)
            query = query.Where(t => t.Status == status);

        var totalCount = await query.CountAsync();

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
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

        _mapper.Map(dto, task);

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
        var query = ApplyUserFilter(_context.Tasks.AsNoTracking())
            .Where(t => t.IsOverdue);

        return await query
            .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<TaskStatsDto> GetStatsAsync()
    {
        var query = ApplyUserFilter(_context.Tasks.AsNoTracking());

        var grouped = await query
            .GroupBy(t => 1)
            .Select(g => new TaskStatsDto
            {
                Total = g.Count(),
                Completed = g.Count(t => t.Status == TaskItemStatus.Completed),
                Pending = g.Count(t => t.Status == TaskItemStatus.Pending),
                InProgress = g.Count(t => t.Status == TaskItemStatus.InProgress)
            })
            .FirstOrDefaultAsync();

        return grouped ?? new TaskStatsDto();
    }
}
