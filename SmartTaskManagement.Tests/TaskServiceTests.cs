using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SmartTaskManagement.Application.DTOs.Task;
using SmartTaskManagement.Application.Exceptions;
using SmartTaskManagement.Application.Interfaces;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;
using SmartTaskManagement.Infrastructure.Data;
using SmartTaskManagement.Infrastructure.Services;
namespace SmartTaskManagement.Tests;

public class TaskServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly IMapper _mapper;
    private readonly TaskService _service;

    private readonly Guid _userId = Guid.NewGuid();

    public TaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(x => x.UserId).Returns(_userId);
        _currentUserMock.Setup(x => x.IsAdmin).Returns(false);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TaskItem, TaskDto>();
        });

        _mapper = config.CreateMapper();

        _service = new TaskService(_context, _currentUserMock.Object, _mapper);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Task()
    {
        var dto = new CreateTaskDto
        {
            Title = "Test Task",
            Description = "Description",
        };

        var taskId = await _service.CreateAsync(dto);

        var task = await _context.Tasks.FindAsync(taskId);

        task.Should().NotBeNull();
        task!.Title.Should().Be("Test Task");
        task.UserId.Should().Be(_userId);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Only_User_Tasks()
    {
        _context.Tasks.AddRange(
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "User Task",
                UserId = _userId
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Other Task",
                UserId = Guid.NewGuid()
            });

        await _context.SaveChangesAsync();

        var result = await _service.GetAllAsync(null, 1, 10);

        result.Data.Should().HaveCount(1);
        result.Data.First().Title.Should().Be("User Task");
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_If_Not_Owner()
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            UserId = Guid.NewGuid()
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var dto = new UpdateTaskDto
        {
            Title = "Updated",
            Description = "Desc",
            Status = TaskItemStatus.Completed,
            Priority = TaskItemPriority.High
        };

        Func<Task> action = async () =>
            await _service.UpdateAsync(task.Id, dto);

        await action.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task DeleteAsync_Should_Soft_Delete_Task()
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            UserId = _userId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        await _service.DeleteAsync(task.Id);

        var deletedTask = await _context.Tasks.FindAsync(task.Id);

        deletedTask!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetStatsAsync_Should_Return_Correct_Counts()
    {
        _context.Tasks.AddRange(
            new TaskItem { Id = Guid.NewGuid(), UserId = _userId, Status = TaskItemStatus.Completed },
            new TaskItem { Id = Guid.NewGuid(), UserId = _userId, Status = TaskItemStatus.Pending },
            new TaskItem { Id = Guid.NewGuid(), UserId = _userId, Status = TaskItemStatus.InProgress }
        );

        await _context.SaveChangesAsync();

        var stats = await _service.GetStatsAsync();

        stats.Total.Should().Be(3);
        stats.Completed.Should().Be(1);
        stats.Pending.Should().Be(1);
        stats.InProgress.Should().Be(1);
    }
}
