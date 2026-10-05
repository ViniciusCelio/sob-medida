import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../../core/material.module';
import { Project } from '../../../../../core/models/profile.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProfileService } from '../../../../../core/services/profile.service';

@Component({
    selector: 'app-project-dialog',
    standalone: true,
    imports: [CommonModule, FormsModule, MaterialModule],
    templateUrl: './project-dialog.html'
})
export class ProjectDialogComponent {
    form: Project;
    isLoading = false;

    constructor(
        public dialogRef: MatDialogRef<ProjectDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: { entry?: Project },
        private profileService: ProfileService,
        private snackBar: MatSnackBar
    ) {
        this.form = data.entry ? { ...data.entry } : {
            name: '', description: '', technologies: '',
            projectUrl: '', gitHubUrl: '', startDate: '', endDate: ''
        };
    }

    cancel(): void {
        this.dialogRef.close();
    }

    save(): void {
        if (this.form.startDate) {
            const startYear = new Date(this.form.startDate).getFullYear();
            if (startYear < 1900 || startYear > 2099) {
                this.snackBar.open('Please enter a valid Start Date.', 'Close', { duration: 3000 });
                return;
            }
        }

        if (this.form.endDate) {
            const endYear = new Date(this.form.endDate).getFullYear();
            if (endYear < 1900 || endYear > 2099) {
                this.snackBar.open('Please enter a valid End Date.', 'Close', { duration: 3000 });
                return;
            }
        }

        if (this.form.startDate && this.form.endDate) {
            if (this.form.startDate > this.form.endDate) {
                this.snackBar.open('End Date cannot be earlier than Start Date.', 'Close', { duration: 3000 });
                return;
            }
        }

        const payload: Project = {
            ...this.form,
            startDate: this.form.startDate
                ? new Date(this.form.startDate).toISOString()
                : null,
            endDate: this.form.endDate
                ? new Date(this.form.endDate).toISOString()
                : null
        };

        const request = this.data.entry?.id
            ? this.profileService.updateProject(this.data.entry.id, payload)
            : this.profileService.createProject(payload);

        this.isLoading = true;
        request.subscribe({
            next: () => {
                this.isLoading = false;
                this.snackBar.open('Project saved successfully.', 'Close', { duration: 3000 });
                this.dialogRef.close(true);
            },
            error: () => {
                this.isLoading = false;
                this.snackBar.open('Failed to save. Please try again.', 'Close', { duration: 3000 });
            }
        });
    }
}