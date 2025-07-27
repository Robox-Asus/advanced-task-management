import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environment/environment';
import { Project, CreateProject, UpdateProject, AddTeamMember } from '../../shared/models/project.models';
import { User } from '../../shared/models/user.models';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private apiUrl = environment.apiUrl + '/Projects';

  constructor(private http: HttpClient) { }

  getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(this.apiUrl);
  }

  getProject(id: number): Observable<Project> {
    return this.http.get<Project>(`${this.apiUrl}/${id}`);
  }

  createProject(project: CreateProject): Observable<Project> {
    return this.http.post<Project>(this.apiUrl, project);
  }

  updateProject(id: number, project: UpdateProject): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, project);
  }

  deleteProject(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  addTeamMember(projectId: number, userId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${projectId}/team-members`, { userId });
  }

  removeTeamMember(projectId: number, userId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${projectId}/team-members/${userId}`);
  }

  // You might need a service to get all users for assigning project managers/team members
  getAllUsers(): Observable<User[]> {
    // Assuming you have an endpoint like /api/Users or /api/Admin/Users
    // You'd need to create this endpoint in your .NET Core API.
    return this.http.get<User[]>(`${environment.apiUrl}/Users`);
  }
}