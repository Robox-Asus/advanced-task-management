using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Models.DTOs;

public class AddTeamMemberDto
{
    [Required]
    public string UserId { get; set; } = string.Empty;
}
