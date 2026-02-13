using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.DTOs.Task;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;
using SmartTaskManagement.Infrastructure.Data;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TasksController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var userId = GetCurrentUserId();

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            UserId = userId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return Ok(task.Id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        TaskItemStatus? status,
        int page = 1,
        int pageSize = 10)
    {
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

        var query = _context.Tasks.AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(t => t.UserId == user!.Id);
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status);
        }

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

        return Ok(new
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Data = tasks
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskDto dto)
    {
        var userId = GetCurrentUserId();

        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

        if (!isAdmin && task.UserId != userId)
            return Forbid();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();

        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

        if (!isAdmin && task.UserId != userId)
            return Forbid();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        return Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
