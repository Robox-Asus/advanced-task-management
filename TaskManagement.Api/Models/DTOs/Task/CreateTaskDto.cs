using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Models.DTOs;

public class CreateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; } = "ToDo"; // Should match TaskStatus enum names
        [Required]
        public string Priority { get; set; } = "Medium"; // Should match TaskPriority enum names
        public DateTime? DueDate { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public string? AssignedToId { get; set; }
    }
