import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectService } from '../../../core/services/project.service';
import { Project } from '../../../shared/models/project.models';
import { User } from '../../../shared/models/user.models';
import { AuthService } from '../../../core/services/auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms'; // For adding team member

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  standalone: false
})
export class ProjectDetailComponent implements OnInit {
  project: Project | null = null;
  loading = true;
  error: string | null = null;
  projectId: number | null = null;
  allUsers: User[] = []; // For adding team members
  addTeamMemberForm: FormGroup;
  addTeamMemberError: string | null = null;
  addTeamMemberSuccess: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private projectService: ProjectService,
    public authService: AuthService,
    private fb: FormBuilder
  ) {
    this.addTeamMemberForm = this.fb.group({
      userId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.projectId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.projectId) {
      this.loadProjectDetails(this.projectId);
      this.loadAllUsers();
    } else {
      this.error = 'Project ID not provided.';
      this.loading = false;
    }
  }

  loadProjectDetails(id: number): void {
    this.loading = true;
    this.error = null;
    this.projectService.getProject(id).subscribe({
      next: (data) => {
        this.project = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load project details:', err);
        this.error = 'Failed to load project details. It might not exist or you lack permission.';
        this.loading = false;
      }
    });
  }

  loadAllUsers(): void {
    // This assumes an /api/Users endpoint exists and is accessible
    this.projectService.getAllUsers().subscribe({
      next: (users) => {
        this.allUsers = users;
      },
      error: (err) => {
        console.error('Failed to load users:', err);
      }
    });
  }

  onAddTeamMemberSubmit(): void {
    this.addTeamMemberError = null;
    this.addTeamMemberSuccess = null;

    if (this.addTeamMemberForm.invalid || !this.projectId) {
      return;
    }

    const userIdToAdd = this.addTeamMemberForm.value.userId;

    this.projectService.addTeamMember(this.projectId, userIdToAdd).subscribe({
      next: () => {
        this.addTeamMemberSuccess = 'Team member added successfully!';
        this.addTeamMemberForm.reset();
        this.loadProjectDetails(this.projectId!); // Reload project to see new member
      },
      error: (err:any) => {
        console.error('Failed to add team member:', err);
        this.addTeamMemberError = err.error?.message || 'Failed to add team member.';
      }
    });
  }

  removeTeamMember(userId: string): void {
    if (!this.projectId || !confirm('Are you sure you want to remove this team member from the project?')) {
      return;
    }

    this.projectService.removeTeamMember(this.projectId, userId).subscribe({
      next: () => {
        console.log('Team member removed successfully!');
        this.loadProjectDetails(this.projectId!); // Reload project to reflect changes
      },
      error: (err) => {
        console.error('Failed to remove team member:', err);
        // Display error message to user
      }
    });
  }

  // Helper to check if a user is already a team member
  isTeamMember(userId: string): boolean {
    return this.project?.teamMembers?.some(member => member.id === userId) || false;
  }
}