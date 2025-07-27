namespace TaskManagement.Api.Models.DTOs;

public class TaskAssigneeDistributionDto
{
    public string AssigneeName { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public int CompletedTaskCount { get; set; }
}
