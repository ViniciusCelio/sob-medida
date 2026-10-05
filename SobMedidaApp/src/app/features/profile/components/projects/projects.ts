import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { MaterialModule } from '../../../../core/material.module';
import { ProfileService } from '../../../../core/services/profile.service';
import { Project } from '../../../../core/models/profile.model';
import { ProjectDialogComponent } from './dialog/project-dialog';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './projects.html'
})
export class ProjectsComponent implements OnInit {
  entries: Project[] = [];

  constructor(
    private profileService: ProfileService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.profileService.getProjects().subscribe({
      next: (data) => {
        this.entries = data;
        this.cdr.detectChanges();
      }
    });
  }

  openDialog(entry?: Project): void {
    const ref = this.dialog.open(ProjectDialogComponent, {
      width: '680px',
      disableClose: true,
      data: { entry }
    });

    ref.afterClosed().subscribe(saved => {
      if (saved) this.load();
    });
  }

  delete(id: number): void {
    if (!confirm('Are you sure you want to delete this project?')) return;

    this.profileService.deleteProject(id).subscribe({
      next: () => {
        this.snackBar.open('Project deleted.', 'Close', { duration: 2000 });
        this.load();
        this.cdr.detectChanges();
      }
    });
  }
}