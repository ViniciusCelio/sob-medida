import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { MaterialModule } from '../../../../core/material.module';
import { ProfileService } from '../../../../core/services/profile.service';
import { Certification } from '../../../../core/models/profile.model';
import { CertificationDialogComponent } from './dialog/certification-dialog';

@Component({
  selector: 'app-certifications',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './certifications.html'
})
export class CertificationsComponent implements OnInit {
  entries: Certification[] = [];

  constructor(
    private profileService: ProfileService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.profileService.getCertifications().subscribe({
      next: (data) => {
        this.entries = data;
        this.cdr.detectChanges();
      }
    });
  }

  openDialog(entry?: Certification): void {
    const ref = this.dialog.open(CertificationDialogComponent, {
      width: '680px',
      disableClose: true,
      data: { entry }
    });

    ref.afterClosed().subscribe(saved => {
      if (saved) this.load();
    });
  }

  delete(id: number): void {
    if (!confirm('Are you sure you want to delete this certification?')) return;
    this.profileService.deleteCertification(id).subscribe({
      next: () => {
        this.snackBar.open('Certification deleted.', 'Close', { duration: 2000 });
        this.load();
        this.cdr.detectChanges();
      }
    });
  }
}