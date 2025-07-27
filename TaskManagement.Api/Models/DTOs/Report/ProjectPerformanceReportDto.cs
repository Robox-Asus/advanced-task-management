namespace TaskManagement.Api.Models.DTOs;

public class ProjectPerformanceReportDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int OverdueTasks { get; set; }
    public double CompletionRate { get; set; }
    public List<TaskAssigneeDistributionDto> TaskDistributionByAssignee { get; set; } = new List<TaskAssigneeDistributionDto>();
}
