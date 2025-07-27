import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  standalone: false
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  loading = false;
  success = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    // Redirect to dashboard if already logged in
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/dashboard']);
    }
  }

  // Custom validator for password match
  passwordMatchValidator(formGroup: FormGroup) {
    const password = formGroup.get('password')?.value;
    const confirmPassword = formGroup.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { mismatch: true };
  }

  onSubmit(): void {
    this.loading = true;
    this.error = '';
    this.success = false;

    if (this.registerForm.invalid) {
      this.loading = false;
      return;
    }

    const { firstName, lastName, email, password, confirmPassword } = this.registerForm.value;

    this.authService.register({ firstName, lastName, email, password, confirmPassword }).subscribe({
      next: () => {
        this.success = true;
        this.loading = false;
        // Optionally, auto-login or redirect to login page after successful registration
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 3000);
      },
      error: err => {
        console.error('Registration error:', err);
        this.error = 'Registration failed. Please check your details and try again.';
        if (err.error && typeof err.error === 'object') {
          // Attempt to parse specific error messages from the backend
          const errors = Object.values(err.error).flat();
          if (errors.length > 0) {
            this.error = errors.join('; ');
          }
        }
        this.loading = false;
      }
    });
  }
}