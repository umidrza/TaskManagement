using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.DTOs.Comment;
using SmartTaskManagement.Application.Interfaces;

namespace SmartTaskManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ITaskCommentService _commentService;

    public CommentController(ITaskCommentService taskService)
    {
        _commentService = taskService;
    }

    [HttpPost("{taskId}")]
    public async Task<IActionResult> Create(Guid taskId, CreateCommentDto dto)
    {
        var commentId = await _commentService.CreateAsync(taskId, dto);
        return Ok(commentId);
    }
}
