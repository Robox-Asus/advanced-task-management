import { Component, OnInit } from '@angular/core';
import { ReportService } from '../../core/services/report.service';
import { ProjectPerformanceReport, BatchUpdateTaskStatus } from '../../shared/models/report.models';
import { Task } from '../../shared/models/task.models'; // For TaskStatus enum
import { TaskService } from '../../core/services/task.service'; // To get tasks for batch update example

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  standalone: false
})
export class ReportComponent implements OnInit {
  projectReports: ProjectPerformanceReport[] = [];
  allTasks: Task[] = []; // For batch update example
  selectedTaskIds: number[] = [];
  batchUpdateStatus: 'ToDo' | 'InProgress' | 'Done' | 'Blocked' = 'ToDo';
  loadingReports = true;
  loadingBatchUpdate = false;
  reportError: string | null = null;
  batchUpdateMessage: string | null = null;
  batchUpdateError: string | null = null;

  constructor(private reportService: ReportService, private taskService: TaskService) { }

  ngOnInit(): void {
    this.loadProjectPerformanceReport();
    this.loadAllTasksForBatchUpdate();
  }

  loadProjectPerformanceReport(): void {
    this.loadingReports = true;
    this.reportError = null;
    this.reportService.getProjectPerformanceReport().subscribe({
      next: (data) => {
        this.projectReports = data;
        this.loadingReports = false;
      },
      error: (err) => {
        console.error('Failed to load project performance report:', err);
        this.reportError = 'Failed to load reports. Please try again later.';
        this.loadingReports = false;
      }
    });
  }

  loadAllTasksForBatchUpdate(): void {
    // This is for demonstration. In a real app, you might fetch tasks for a specific project
    // or implement a more robust task selection mechanism.
    this.taskService.getTasks().subscribe({
      next: (tasks) => {
        this.allTasks = tasks;
      },
      error: (err) => {
        console.error('Failed to load tasks for batch update:', err);
      }
    });
  }

  toggleTaskSelection(taskId: number): void {
    const index = this.selectedTaskIds.indexOf(taskId);
    if (index > -1) {
      this.selectedTaskIds.splice(index, 1);
    } else {
      this.selectedTaskIds.push(taskId);
    }
  }

  performBatchUpdate(): void {
    this.loadingBatchUpdate = true;
    this.batchUpdateMessage = null;
    this.batchUpdateError = null;

    if (this.selectedTaskIds.length === 0) {
      this.batchUpdateError = 'Please select at least one task for batch update.';
      this.loadingBatchUpdate = false;
      return;
    }

    const payload: BatchUpdateTaskStatus = {
      taskIds: this.selectedTaskIds,
      newStatus: this.batchUpdateStatus
    };

    this.reportService.batchUpdateTaskStatus(payload).subscribe({
      next: (res) => {
        this.batchUpdateMessage = res.message || 'Batch update successful!';
        this.selectedTaskIds = []; // Clear selection
        this.loadProjectPerformanceReport(); // Reload reports to reflect changes
        this.loadAllTasksForBatchUpdate(); // Reload tasks to reflect changes
        this.loadingBatchUpdate = false;
      },
      error: (err) => {
        console.error('Batch update failed:', err);
        this.batchUpdateError = err.error?.message || 'Batch update failed. Please check the console for details.';
        this.loadingBatchUpdate = false;
      }
    });
  }
}