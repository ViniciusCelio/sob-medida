import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MaterialModule } from '../../core/material.module';
import { AuthService } from '../../core/services/auth.service';
import { ProfileService } from '../../core/services/profile.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink, MaterialModule],
  templateUrl: './dashboard.html'
})
export class DashboardComponent implements OnInit {
  fullName = '';

  constructor(
    private authService: AuthService,
    private profileService: ProfileService,
    private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.cdr.markForCheck()
    this.profileService.profile$.subscribe({
      next: (profile) => {
        if (profile && profile.fullName) {
          this.fullName = profile.fullName;
          this.cdr.markForCheck();
        }
      }
    });

    this.profileService.getPersonalInfo().subscribe({
        error: () => {
            this.fullName = this.authService.getFullName();
            this.cdr.markForCheck();
        }
    });
  }

  logout(): void {
    this.authService.logout();
  }

  get initials(): string {
    if (!this.fullName) return '';
    this.cdr.markForCheck();

    return this.fullName
      .split(' ')
      .slice(0, 2)
      .map(n => n[0])
      .join('')
      .toUpperCase();
  }

}