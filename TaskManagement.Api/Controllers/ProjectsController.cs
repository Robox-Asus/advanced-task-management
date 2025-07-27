using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Models.DTOs;
using TaskManagement.Api.Services;
using System.Security.Claims; // For getting user ID

namespace TaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All project endpoints require authentication by default
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        [Authorize(Policy = "TeamMemberOrHigher")] // Any authenticated user can view projects
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "TeamMemberOrHigher")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [HttpPost]
        [Authorize(Policy = "ProjectManagerOrAdmin")] // Only Project Managers or Admins can create projects
        public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectDto createProjectDto)
        {
            // Optionally, if ProjectManagerId is not provided, set the current user as manager
            if (string.IsNullOrEmpty(createProjectDto.ProjectManagerId))
            {
                createProjectDto.ProjectManagerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var project = await _projectService.CreateProjectAsync(createProjectDto);
            // In a real app, you'd map the created project entity back to a DTO for the response
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "ProjectManagerOrAdmin")] // Only Project Managers or Admins can update projects
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto updateProjectDto)
        {
            var success = await _projectService.UpdateProjectAsync(id, updateProjectDto);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")] // Only Admins can delete projects
        public async Task<IActionResult> DeleteProject(int id)
        {
            var success = await _projectService.DeleteProjectAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{projectId}/team-members")]
        [Authorize(Policy = "ProjectManagerOrAdmin")]
        public async Task<IActionResult> AddTeamMember(int projectId, [FromBody] AddTeamMemberDto model)
        {
            var success = await _projectService.AddTeamMemberToProjectAsync(projectId, model.UserId);
            if (!success)
            {
                return BadRequest("Failed to add team member or user already a member.");
            }
            return Ok(new { Message = "Team member added successfully." });
        }

        [HttpDelete("{projectId}/team-members/{userId}")]
        [Authorize(Policy = "ProjectManagerOrAdmin")]
        public async Task<IActionResult> RemoveTeamMember(int projectId, string userId)
        {
            var success = await _projectService.RemoveTeamMemberFromProjectAsync(projectId, userId);
            if (!success)
            {
                return NotFound("Team member not found in project or project not found.");
            }
            return NoContent();
        }
    }
}