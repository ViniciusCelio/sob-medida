import { Component, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { MaterialModule } from '../../../core/material.module';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink, MaterialModule],
  templateUrl: './login.html'
})
export class LoginComponent {
  email = '';
  password = '';
  errorMessage = '';
  isLoading = false;
  hidePassword = true;

  constructor(private authService: AuthService, private router: Router, private cdr : ChangeDetectorRef) {}

  onSubmit(): void {
    this.errorMessage = '';
    this.isLoading = true;

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Login falhou. Por favor, tente novamente.';
        this.cdr.markForCheck();
      }
    });
  }
}