import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskService } from '../../../core/services/task.service';
import { Task } from '../../../shared/models/task.models';
import { AuthService } from '../../../core/services/auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-task-detail',
  templateUrl: './task-detail.component.html',
  standalone: false
})
export class TaskDetailComponent implements OnInit {
  task: Task | null = null;
  loading = true;
  error: string | null = null;
  taskId: number | null = null;
  commentForm: FormGroup;
  commentError: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private taskService: TaskService,
    public authService: AuthService,
    private fb: FormBuilder
  ) {
    this.commentForm = this.fb.group({
      content: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.taskId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.taskId) {
      this.loadTaskDetails(this.taskId);
    } else {
      this.error = 'Task ID not provided.';
      this.loading = false;
    }
  }

  loadTaskDetails(id: number): void {
    this.loading = true;
    this.error = null;
    this.taskService.getTask(id).subscribe({
      next: (data) => {
        this.task = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load task details:', err);
        this.error = 'Failed to load task details. It might not exist or you lack permission.';
        this.loading = false;
      }
    });
  }

  onAddCommentSubmit(): void {
    this.commentError = null;
    if (this.commentForm.invalid || !this.taskId) {
      return;
    }

    const content = this.commentForm.value.content;
    this.taskService.addCommentToTask(this.taskId, { content }).subscribe({
      next: (newComment) => {
        if (this.task?.comments) {
          this.task.comments.push(newComment); // Add new comment to the list
        } else {
          this.task!.comments = [newComment]; // Initialize if null
        }
        this.commentForm.reset(); // Clear the form
      },
      error: (err) => {
        console.error('Failed to add comment:', err);
        this.commentError = err.error?.message || 'Failed to add comment.';
      }
    });
  }

  deleteTask(id: number): void {
    if (confirm('Are you sure you want to delete this task?')) {
      this.taskService.deleteTask(id).subscribe({
        next: () => {
          console.log('Task deleted successfully!');
          this.router.navigate(['/projects', this.task?.projectId]); // Navigate back to project details
        },
        error: (err) => {
          console.error('Error deleting task:', err);
          this.error = 'Failed to delete task.';
        }
      });
    }
  }
}