using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;
using TaskManagement.Api.Models.DTOs;

namespace TaskManagement.Api.Services
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
        {
            return await _context.TaskItems
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    CreatedDate = t.CreatedDate,
                    DueDate = t.DueDate,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : "Unassigned"
                })
                .ToListAsync();
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int id)
        {
            return await _context.TaskItems
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                    .ThenInclude(tc => tc.User)
                .Where(t => t.Id == id)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    CreatedDate = t.CreatedDate,
                    DueDate = t.DueDate,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : "Unassigned",
                    Comments = t.Comments.Select(c => new TaskCommentDto
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedDate = c.CreatedDate,
                        UserId = c.UserId,
                        UserName = $"{c.User.FirstName} {c.User.LastName}"
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TaskItem> CreateTaskAsync(CreateTaskDto createTaskDto)
        {
            var task = new TaskItem
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                Status = Enum.Parse<TaskStatusAPI>(createTaskDto.Status),
                Priority = Enum.Parse<TaskPriority>(createTaskDto.Priority),
                DueDate = createTaskDto.DueDate,
                ProjectId = createTaskDto.ProjectId,
                AssignedToId = createTaskDto.AssignedToId,
                CreatedDate = DateTime.UtcNow
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.Status = Enum.Parse<TaskStatusAPI>(updateTaskDto.Status);
            task.Priority = Enum.Parse<TaskPriority>(updateTaskDto.Priority);
            task.DueDate = updateTaskDto.DueDate;
            task.AssignedToId = updateTaskDto.AssignedToId;

            _context.TaskItems.Update(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TaskComment> AddCommentToTaskAsync(int taskId, AddTaskCommentDto commentDto, string userId)
        {
            var task = await _context.TaskItems.FindAsync(taskId);
            if (task == null)
            {
                throw new ArgumentException("Task not found.");
            }

            var comment = new TaskComment
            {
                Content = commentDto.Content,
                TaskItemId = taskId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            _context.TaskComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }
    }
}