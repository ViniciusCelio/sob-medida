import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { MaterialModule } from '../../../../core/material.module';
import { ProfileService } from '../../../../core/services/profile.service';
import { Education } from '../../../../core/models/profile.model';
import { EducationDialogComponent } from './dialog/education-dialog';

@Component({
  selector: 'app-education',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './education.html'
})
export class EducationComponent implements OnInit {
  entries: Education[] = [];

  constructor(
    private profileService: ProfileService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.profileService.getEducations().subscribe({
      next: (data) => {
        this.entries = data;
        this.cdr.detectChanges();
      }
    });
  }

  openDialog(entry?: Education): void {
    const ref = this.dialog.open(EducationDialogComponent, {
      width: '680px',
      disableClose: true,
      data: { entry }
    });

    ref.afterClosed().subscribe(saved => {
      if (saved) this.load();
    });
  }

  delete(id: number): void {
    if (!confirm('Você tem certeza que deseja deletar essa entrada?')) return;
    this.profileService.deleteEducation(id).subscribe({
      next: () => {
        this.snackBar.open('Deletado.', 'Close', { duration: 2000 });
        this.load();
        this.cdr.detectChanges();
      }
    });
  }
}