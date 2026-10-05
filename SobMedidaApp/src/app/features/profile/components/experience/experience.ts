import { Component, OnInit, ChangeDetectorRef } from '@angular/core'; // 1. Added ChangeDetectorRef
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { MaterialModule } from '../../../../core/material.module';
import { ProfileService } from '../../../../core/services/profile.service';
import { Experience } from '../../../../core/models/profile.model';
import { ExperienceDialogComponent } from './dialog/experience-dialog';

@Component({
  selector: 'app-experience',
  standalone: true,
  imports: [CommonModule, FormsModule, MaterialModule],
  templateUrl: './experience.html'
})
export class ExperienceComponent implements OnInit {
  entries: Experience[] = [];
  editingId: number | null = null;
  showForm = false;
  isLoading = false;

  constructor(
    private profileService: ProfileService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.profileService.getExperiences().subscribe({
      next: (data) => {
        this.entries = data;
        this.cdr.detectChanges();
      }
    });
  }

  openDialog(entry?: Experience): void {
    const ref = this.dialog.open(ExperienceDialogComponent, {
      width: '680px',
      disableClose: true,
      data: { entry }
    });

    ref.afterClosed().subscribe(saved => {
      if (saved) this.load();
    });
  }

  delete(id: number): void {
    if (!confirm('Are you sure you want to delete this entry?')) return;
    this.profileService.deleteExperience(id).subscribe({
      next: () => {
        this.snackBar.open('Entry deleted.', 'Close', { duration: 2000 });
        this.load();
        this.cdr.detectChanges();
      }
    });
  }
}