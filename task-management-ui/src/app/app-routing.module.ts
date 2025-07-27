import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';
import { ProjectListComponent } from './features/projects/project-list/project-list.component';
import { ProjectDetailComponent } from './features/projects/project-detail/project-detail.component';
import { TaskDetailComponent } from './features/tasks/task-detail/task-detail.component';
import { ReportComponent } from './features/reports/report.component';

const routes: Routes = [ // No longer exported as a constant for external use
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  {
    path: 'projects',
    component: ProjectListComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'ProjectManager', 'TeamMember'] }
  },
  {
    path: 'projects/:id',
    component: ProjectDetailComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'ProjectManager', 'TeamMember'] }
  },
  {
    path: 'tasks/:id',
    component: TaskDetailComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'ProjectManager', 'TeamMember'] }
  },
  {
    path: 'reports',
    component: ReportComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'ProjectManager'] } // Only PMs and Admins can view reports
  },
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: '/dashboard' } // Wildcard route for any unmatched URL
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }