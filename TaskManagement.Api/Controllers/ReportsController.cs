using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Models;
using TaskManagement.Api.Models.DTOs;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ProjectManagerOrAdmin")] // Only Project Managers and Admins can access reports
    public class ReportsController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Generates a comprehensive project performance report using parallel processing.
        /// This endpoint is designed to handle potentially high CPU-bound load for large datasets.
        /// </summary>
        [HttpGet("project-performance")]
        public async Task<ActionResult<IEnumerable<ProjectPerformanceReportDto>>> GetProjectPerformanceReport()
        {
            var reports = await _reportService.GenerateProjectPerformanceReportAsync();
            return Ok(reports);
        }

        /// <summary>
        /// Performs a batch update of task statuses, demonstrating concurrent I/O operations.
        /// </summary>
        [HttpPost("batch-update-task-status")]
        [Authorize(Policy = "ProjectManagerOrAdmin")] // Only Project Managers and Admins can perform batch updates
        public async Task<IActionResult> BatchUpdateTaskStatus([FromBody] BatchUpdateTaskStatusDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!System.Enum.TryParse(model.NewStatus, true, out TaskStatusAPI newStatusEnum))
            {
                return BadRequest("Invalid task status provided.");
            }

            var success = await _reportService.BatchUpdateTaskStatusAsync(model.TaskIds, newStatusEnum);

            if (!success)
            {
                return BadRequest("Failed to perform batch update. Some tasks might not exist.");
            }

            return Ok(new { Message = $"Successfully updated status for {model.TaskIds.Count} tasks to {model.NewStatus}." });
        }
    }
}