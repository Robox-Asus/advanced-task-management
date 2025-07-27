namespace TaskManagement.Api.Models.DTOs;

public class BatchUpdateTaskStatusDto
{
    public List<int> TaskIds { get; set; } = new List<int>();
    public string NewStatus { get; set; } = string.Empty; // Should match TaskStatus enum names
}
