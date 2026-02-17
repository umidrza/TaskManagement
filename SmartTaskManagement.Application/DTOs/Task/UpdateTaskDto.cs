using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Application.DTOs.Task;

public class UpdateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; }
    public TaskItemPriority Priority { get; set; }
}