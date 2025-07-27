using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Models.DTOs;

public class CreateProjectDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public string? ProjectManagerId { get; set; } // ID of the user who will manage the project
    }
