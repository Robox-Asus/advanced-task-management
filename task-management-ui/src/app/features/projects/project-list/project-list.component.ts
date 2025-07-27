import { Component, OnInit } from '@angular/core';
import { ProjectService } from '../../../core/services/project.service';
import { Project } from '../../../shared/models/project.models';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.component.html',
  standalone: false
})
export class ProjectListComponent implements OnInit {
  projects: Project[] = [];
  loading = true;
  error: string | null = null;

  constructor(private projectService: ProjectService, public authService: AuthService) { }

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading = true;
    this.error = null;
    this.projectService.getProjects().subscribe({
      next: (data) => {
        this.projects = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load projects:', err);
        this.error = 'Failed to load projects. Please try again later.';
        this.loading = false;
      }
    });
  }

  deleteProject(id: number): void {
    if (confirm('Are you sure you want to delete this project? This action cannot be undone.')) {
      this.projectService.deleteProject(id).subscribe({
        next: () => {
          this.projects = this.projects.filter(p => p.id !== id);
          // Show a success message (e.g., using a toast notification)
          console.log('Project deleted successfully!');
        },
        error: (err) => {
          console.error('Error deleting project:', err);
          this.error = 'Failed to delete project.';
        }
      });
    }
  }
}