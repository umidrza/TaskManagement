using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.DTOs.Task;
using SmartTaskManagement.Application.Interfaces;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var taskId = await _taskService.CreateAsync(dto);
        return Ok(taskId);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        TaskItemStatus? status,
        int page = 1,
        int pageSize = 10)
    {
        var result = await _taskService.GetAllAsync(status, page, pageSize);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskDto dto)
    {
        await _taskService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _taskService.DeleteAsync(id);
        return NoContent();
    }
}
