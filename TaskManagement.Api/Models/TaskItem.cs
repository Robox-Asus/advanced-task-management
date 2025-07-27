using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Models;

public enum TaskStatusAPI
{
    ToDo,
    InProgress,
    Done,
    Blocked
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical
}

public class TaskItem
{
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatusAPI Status { get; set; } = TaskStatusAPI.ToDo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }

    // Foreign key for Project
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!; // Required navigation property

    // Foreign key for Assigned User (ApplicationUser)
    public string? AssignedToId { get; set; }
    public ApplicationUser? AssignedTo { get; set; }

    // Navigation property for comments
    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
}