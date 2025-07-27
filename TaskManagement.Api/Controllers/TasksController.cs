using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Models.DTOs;
using TaskManagement.Api.Services;
using System.Security.Claims; // For getting user ID

namespace TaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All task endpoints require authentication by default
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskService;

        public TasksController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        [Authorize(Policy = "TeamMemberOrHigher")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "TeamMemberOrHigher")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost]
        [Authorize(Policy = "ProjectManagerOrAdmin")] // Only Project Managers or Admins can create tasks
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var task = await _taskService.CreateTaskAsync(createTaskDto);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "TeamMemberOrHigher")] // Team members can update their assigned tasks, PMs/Admins can update all
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            // Implement more granular authorization here if needed, e.g.,
            // check if the current user is the assigned user, or a PM/Admin.
            // For simplicity, we'll rely on the policy for now.
            var success = await _taskService.UpdateTaskAsync(id, updateTaskDto);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ProjectManagerOrAdmin")] // Only Project Managers or Admins can delete tasks
        public async Task<IActionResult> DeleteTask(int id)
        {
            var success = await _taskService.DeleteTaskAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{taskId}/comments")]
        [Authorize(Policy = "TeamMemberOrHigher")]
        public async Task<ActionResult<TaskCommentDto>> AddComment(int taskId, [FromBody] AddTaskCommentDto commentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            try
            {
                var comment = await _taskService.AddCommentToTaskAsync(taskId, commentDto, userId);
                // Map to DTO for response
                return CreatedAtAction(nameof(GetTask), new { id = taskId }, comment);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}