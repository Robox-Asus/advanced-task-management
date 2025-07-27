using Microsoft.AspNetCore.Identity;

namespace TaskManagement.Api.Models;

public class ApplicationUser : IdentityUser
{
    // Add any custom properties for your user here, e.g.,
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // Navigation property for tasks assigned to this user
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}