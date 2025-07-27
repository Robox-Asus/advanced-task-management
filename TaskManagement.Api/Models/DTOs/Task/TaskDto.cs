namespace TaskManagement.Api.Models.DTOs;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Enum as string
    public string Priority { get; set; } = string.Empty; // Enum as string
    public DateTime CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public List<TaskCommentDto> Comments { get; set; } = new List<TaskCommentDto>(); // For detailed view
}
