import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { MaterialModule } from '../../../core/material.module';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, MaterialModule],
  templateUrl: './register.html'
})
export class RegisterComponent {
  fullName = '';
  email = '';
  password = '';
  errorMessage = '';
  successMessage = '';
  isLoading = false;
  hidePassword = true;

  constructor(private authService: AuthService, private router: Router, private cdr : ChangeDetectorRef) {}

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.isLoading = true;

    this.authService.register({
      fullName: this.fullName,
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = 'Account created! Redirecting to login...';
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.isLoading = false;
        if (err.error?.errors) {
          this.errorMessage = Object.values(err.error.errors).flat().join(' ');
          this.cdr.markForCheck();
        } else {
          this.errorMessage = err.error?.message || 'Registro falhou. Por favor, tente novamente.';
          this.cdr.markForCheck();
        }
      }
    });
  }
}