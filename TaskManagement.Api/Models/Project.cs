namespace TaskManagement.Api.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }

    // Foreign key for the Project Manager (ApplicationUser)
    public string? ProjectManagerId { get; set; }
    public ApplicationUser? ProjectManager { get; set; }

    // Navigation property for tasks within this project
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

    // Navigation property for team members in this project (many-to-many relationship)
    public ICollection<ProjectTeamMember> ProjectTeamMembers { get; set; } = new List<ProjectTeamMember>();
}