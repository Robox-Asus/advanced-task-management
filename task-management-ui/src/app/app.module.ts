import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms'; 
import { CommonModule, DatePipe } from '@angular/common'; 
import { RouterLink } from '@angular/router'; 
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ProjectListComponent } from './features/projects/project-list/project-list.component';
import { ProjectDetailComponent } from './features/projects/project-detail/project-detail.component';
import { TaskDetailComponent } from './features/tasks/task-detail/task-detail.component';
import { NavBarComponent } from './shared/components/nav-bar/nav-bar.component';
import { ReportComponent } from './features/reports/report.component';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

@NgModule({
  declarations: [
    AppComponent,
    NavBarComponent,
    LoginComponent,
    RegisterComponent,
    DashboardComponent,
    ProjectListComponent,
    ProjectDetailComponent,
    TaskDetailComponent,
    NavBarComponent,
    ReportComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule, // Keep FormsModule here
    ReactiveFormsModule, // Keep ReactiveFormsModule here
    CommonModule, // For NgIf, NgFor, etc.
    RouterLink, // For routerLink directive
    // DatePipe is usually provided by CommonModule, but if used directly in templates, ensure it's available.
    // Generally, you don't need to import DatePipe explicitly into AppModule if CommonModule is imported.
    // If you were using custom pipes, you'd declare them here.
  ],
  providers: [
    // Provide the AuthInterceptor to automatically add JWT to requests
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    provideHttpClient(withInterceptorsFromDi()),
    DatePipe // Provide DatePipe if it's used directly in component templates that are not standalone
  ],
  bootstrap: [AppComponent] // Bootstrap AppComponent via NgModule
})
export class AppModule { }