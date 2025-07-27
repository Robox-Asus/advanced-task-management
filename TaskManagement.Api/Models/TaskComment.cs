using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Models;

public class TaskComment
{
    public int Id { get; set; }
    [Required]
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Foreign key for the user who made the comment
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!; // Required navigation property

    // Foreign key for the task the comment belongs to
    public int TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!; // Required navigation property
}