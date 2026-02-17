using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; private set; }

    public bool IsDeleted { get; set; } = false;
    public bool IsArchived { get; set; } = false;
    public bool IsOverdue =>
        DueDate.HasValue &&
        DueDate.Value < DateTime.UtcNow &&
        Status != TaskItemStatus.Completed;

    public ICollection<TaskComment> Comments { get; set; } = [];
}