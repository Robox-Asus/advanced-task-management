export interface Task {
  id: number;
  title: string;
  description: string;
  status: 'ToDo' | 'InProgress' | 'Done' | 'Blocked';
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  createdDate: Date;
  dueDate?: Date;
  projectId: number;
  projectName: string;
  assignedToId?: string;
  assignedToName?: string;
  comments?: TaskComment[]; // Nested comments for detail view
}

export interface CreateTask {
  title: string;
  description: string;
  status: 'ToDo' | 'InProgress' | 'Done' | 'Blocked';
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  dueDate?: Date;
  projectId: number;
  assignedToId?: string;
}

export interface UpdateTask {
  title: string;
  description: string;
  status: 'ToDo' | 'InProgress' | 'Done' | 'Blocked';
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  dueDate?: Date;
  assignedToId?: string;
}

export interface TaskComment {
  id: number;
  content: string;
  createdDate: Date;
  userId: string;
  userName: string;
}

export interface AddTaskComment {
  content: string;
}
