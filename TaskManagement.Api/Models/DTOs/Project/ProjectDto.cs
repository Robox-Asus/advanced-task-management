namespace TaskManagement.Api.Models.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? ProjectManagerId { get; set; }
    public string? ProjectManagerName { get; set; }
    public List<TaskDto> Tasks { get; set; } = new List<TaskDto>(); // For detailed view
    public List<UserDto> TeamMembers { get; set; } = new List<UserDto>(); // For detailed view
}
