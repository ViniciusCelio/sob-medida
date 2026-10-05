import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { MaterialModule } from '../../../../core/material.module';
import { ProfileService } from '../../../../core/services/profile.service';
import { Skill } from '../../../../core/models/profile.model';
import { SkillDialogComponent } from './dialog/skill-dialog';

@Component({
  selector: 'app-skills',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './skills.html'
})
export class SkillsComponent implements OnInit {
  entries: Skill[] = [];

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
    this.profileService.getSkills().subscribe({
      next: (data) => {
        this.entries = data;
        this.cdr.detectChanges();
      }
    });
  }

  openDialog(entry?: Skill): void {
    const ref = this.dialog.open(SkillDialogComponent, {
      width: '680px',
      disableClose: true,
      data: { entry }
    });

    ref.afterClosed().subscribe(saved => {
      if (saved) this.load();
    });
  }

  delete(id: number): void {
    if (!confirm('Are you sure you want to delete this skill?')) return;

    this.profileService.deleteSkill(id).subscribe({
      next: () => {
        this.snackBar.open('Skill deleted.', 'Close', { duration: 2000 });
        this.load();
        this.cdr.detectChanges();
      }
    });
  }
}