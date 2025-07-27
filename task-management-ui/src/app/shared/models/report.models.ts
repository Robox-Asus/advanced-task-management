export interface ProjectPerformanceReport {
  projectId: number;
  projectName: string;
  totalTasks: number;
  completedTasks: number;
  inProgressTasks: number;
  overdueTasks: number;
  completionRate: number;
  taskDistributionByAssignee: TaskAssigneeDistribution[];
}

export interface TaskAssigneeDistribution {
  assigneeName: string;
  taskCount: number;
  completedTaskCount: number;
}

export interface BatchUpdateTaskStatus {
  taskIds: number[];
  newStatus: 'ToDo' | 'InProgress' | 'Done' | 'Blocked';
}