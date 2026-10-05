import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../../core/material.module';
import { Skill } from '../../../../../core/models/profile.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProfileService } from '../../../../../core/services/profile.service';

@Component({
    selector: 'app-skill-dialog',
    standalone: true,
    imports: [CommonModule, FormsModule, MaterialModule],
    templateUrl: './skill-dialog.html'
})
export class SkillDialogComponent {
    form: Skill;
    levels = ['Beginner', 'Intermediate', 'Advanced', 'Expert'];
    isLoading = false;

    constructor(
        public dialogRef: MatDialogRef<SkillDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: { entry?: Skill },
        private profileService: ProfileService,
        private snackBar: MatSnackBar
    ) {
        this.form = data.entry ? { ...data.entry } : {
            name: '', category: '', level: ''
        };
    }

    cancel(): void {
        this.dialogRef.close();
    }

    save(): void {
        const request = this.data.entry?.id
            ? this.profileService.updateSkill(this.data.entry.id, this.form)
            : this.profileService.createSkill(this.form);

        this.isLoading = true;
        request.subscribe({
            next: () => {
                this.isLoading = false;
                this.snackBar.open('Skill saved successfully.', 'Close', { duration: 3000 });
                this.dialogRef.close(true);
            },
            error: () => {
                this.isLoading = false;
                this.snackBar.open('Failed to save. Please try again.', 'Close', { duration: 3000 });
            }
        });
    }
}