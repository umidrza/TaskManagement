namespace SmartTaskManagement.Application.DTOs.Task;

public class TaskStatsDto
{
    public int Total { get; set; }
    public int Completed { get; set; }
    public int Pending { get; set; }
    public int InProgress { get; set; }
}
