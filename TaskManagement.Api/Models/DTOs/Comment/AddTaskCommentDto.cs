using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Models.DTOs;

public class AddTaskCommentDto
{
    [Required]
    public string Content { get; set; } = string.Empty;
}
