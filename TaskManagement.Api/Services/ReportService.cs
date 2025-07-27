using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;
using TaskManagement.Api.Models.DTOs;
using System.Collections.Concurrent; // For thread-safe collection

namespace TaskManagement.Api.Services
{
    public class ReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Generates a comprehensive project performance report,
        /// demonstrating parallel processing for potentially large datasets.
        /// </summary>
        /// <returns>A list of ProjectPerformanceReportDto.</returns>
        public async Task<IEnumerable<ProjectPerformanceReportDto>> GenerateProjectPerformanceReportAsync()
        {
            // Fetch all projects and related tasks efficiently.
            // Using AsNoTracking() for read-only operations for performance.
            var projects = await _context.Projects
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.AssignedTo)
                .AsNoTracking()
                .ToListAsync();

            // Use ConcurrentBag for thread-safe addition of results from parallel tasks.
            var reports = new ConcurrentBag<ProjectPerformanceReportDto>();

            // Parallel.ForEach is excellent for CPU-bound operations on collections.
            // It automatically manages threads from the ThreadPool.
            // For I/O-bound operations (like multiple database calls), async/await with Task.WhenAll is preferred.
            // Here, we're simulating CPU-bound calculations per project.
            Parallel.ForEach(projects, project =>
            {
                var totalTasks = project.Tasks.Count;
                var completedTasks = project.Tasks.Count(t => t.Status == TaskStatusAPI.Done);
                var inProgressTasks = project.Tasks.Count(t => t.Status == TaskStatusAPI.InProgress);
                var overdueTasks = project.Tasks.Count(t => t.Status != TaskStatusAPI.Done && t.DueDate.HasValue && t.DueDate.Value < System.DateTime.UtcNow);

                // Calculate task distribution by assignee
                var taskDistribution = project.Tasks
                    .GroupBy(t => t.AssignedTo?.UserName ?? "Unassigned")
                    .Select(g => new TaskAssigneeDistributionDto
                    {
                        AssigneeName = g.Key,
                        TaskCount = g.Count(),
                        CompletedTaskCount = g.Count(t => t.Status == TaskStatusAPI.Done)
                    })
                    .ToList();

                reports.Add(new ProjectPerformanceReportDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    InProgressTasks = inProgressTasks,
                    OverdueTasks = overdueTasks,
                    CompletionRate = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0,
                    TaskDistributionByAssignee = taskDistribution
                });
            });

            return reports.OrderBy(r => r.ProjectName).ToList();
        }

        /// <summary>
        /// Simulates a batch update operation for tasks, using Task.WhenAll for concurrent I/O operations.
        /// This is more suitable for database updates than Parallel.ForEach.
        /// </summary>
        /// <param name="taskIds">List of task IDs to update.</param>
        /// <param name="newStatus">The new status to set for all tasks.</param>
        /// <returns>True if all updates were attempted, false otherwise.</returns>
        public async Task<bool> BatchUpdateTaskStatusAsync(IEnumerable<int> taskIds, TaskStatusAPI newStatus)
        {
            var tasksToUpdate = await _context.TaskItems
                                            .Where(t => taskIds.Contains(t.Id))
                                            .ToListAsync();

            if (!tasksToUpdate.Any())
            {
                return false;
            }

            // Create a list of tasks (representing asynchronous operations)
            var updateTasks = new List<Task>();

            foreach (var task in tasksToUpdate)
            {
                // Each update is an async operation. We don't await immediately.
                // Instead, we add the Task to a list.
                updateTasks.Add(Task.Run(() =>
                {
                    task.Status = newStatus;
                    // In a real-world scenario, you might want to save changes per task
                    // or batch them more efficiently if the ORM supports it.
                    // For simplicity, we'll just update the entity here and save all at once later.
                    // Note: This specific usage of Task.Run inside a loop for EF Core updates
                    // might lead to context issues if not handled carefully (e.g., separate contexts per task).
                    // For true high-performance batch updates, consider bulk update libraries or stored procedures.
                    // This example primarily demonstrates Task.WhenAll for concurrent operations.
                }));
            }

            // Wait for all update tasks to complete.
            await Task.WhenAll(updateTasks);

            // Save all changes to the database. This is a single transaction.
            await _context.SaveChangesAsync();

            return true;
        }
    }
}