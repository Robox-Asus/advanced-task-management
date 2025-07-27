import { Task } from "./task.models";
import { User } from "./user.models";

export interface Project {
  id: number;
  name: string;
  description: string;
  createdDate: Date;
  dueDate?: Date;
  projectManagerId?: string;
  projectManagerName?: string;
  tasks?: Task[]; // Nested tasks for detail view
  teamMembers?: User[]; // Nested team members for detail view
}

export interface CreateProject {
  name: string;
  description: string;
  dueDate?: Date;
  projectManagerId?: string;
}

export interface UpdateProject {
  name: string;
  description: string;
  dueDate?: Date;
  projectManagerId?: string;
}

export interface AddTeamMember {
  userId: string;
}