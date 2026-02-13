using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.DTOs.Task;
using SmartTaskManagement.Application.Interfaces;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly UserManager<ApplicationUser> _userManager;

    public TasksController(
        ITaskService taskService,
        UserManager<ApplicationUser> userManager)
    {
        _taskService = taskService;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var userId = GetUserId();
        var id = await _taskService.CreateAsync(userId, dto);
        return Ok(id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        TaskItemStatus? status,
        int page = 1,
        int pageSize = 10)
    {
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

        var result = await _taskService.GetAllAsync(
            user!.Id,
            isAdmin,
            status,
            page,
            pageSize);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

        await _taskService.UpdateAsync(user!.Id, isAdmin, id, dto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

        await _taskService.DeleteAsync(user!.Id, isAdmin, id);

        return NoContent();
    }

    private Guid GetUserId()
    {
        return Guid.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!
                .Value);
    }
}
