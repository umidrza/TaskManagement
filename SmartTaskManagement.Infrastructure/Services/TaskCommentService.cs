using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.DTOs.Comment;
using SmartTaskManagement.Application.Interfaces;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Services;

public class TaskCommentService : ITaskCommentService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public TaskCommentService(
        ApplicationDbContext context,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(Guid taskId, CreateCommentDto dto)
    {
        var comment = _mapper.Map<TaskComment>(dto);
        comment.Id = Guid.NewGuid();
        comment.UserId = _currentUser.UserId;
        comment.TaskId = taskId;


        _context.TaskComments.Add(comment);
        await _context.SaveChangesAsync();

        return comment.Id;
    }

}
