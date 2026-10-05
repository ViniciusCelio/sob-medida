import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../../core/material.module';
import { Education } from '../../../../../core/models/profile.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProfileService } from '../../../../../core/services/profile.service';

@Component({
    selector: 'app-education-dialog',
    standalone: true,
    imports: [CommonModule, FormsModule, MaterialModule],
    templateUrl: './education-dialog.html'
})
export class EducationDialogComponent {
    form: Education;
    isLoading = false;

    constructor(
        public dialogRef: MatDialogRef<EducationDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: { entry?: Education },
        private profileService: ProfileService,
        private snackBar: MatSnackBar
    ) {
        this.form = data.entry ? { ...data.entry } : {
            degree: '', fieldOfStudy: '', institution: '',
            location: '', startDate: '', endDate: '',
            isCurrentlyStudying: false, description: ''
        };
    }

    onCurrentlyStudyingChange(): void {
        if (this.form.isCurrentlyStudying) {
            this.form.endDate = '';
        }
    }

    cancel(): void {
        this.dialogRef.close();
    }

    save(): void {
        if (this.form.startDate) {
            const startYear = new Date(this.form.startDate).getFullYear();
            if (startYear < 1900 || startYear > 2099) {
                this.snackBar.open('Por favor, selecione uma data de início válida.', 'Close', { duration: 3000 });
                return;
            }
        }

        if (!this.form.isCurrentlyStudying && this.form.endDate) {
            const endYear = new Date(this.form.endDate).getFullYear();
            if (endYear < 1900 || endYear > 2099) {
                this.snackBar.open('Por favor, selecione uma data de fim válida.', 'Close', { duration: 3000 });
                return;
            }
        }

        if (this.form.startDate && this.form.endDate) {
            if (this.form.startDate > this.form.endDate) {
                this.snackBar.open('Data fim não pode ser mais recente que a data de início.', 'Close', { duration: 3000 });
                return;
            }
        }

        const payload: Education = {
            ...this.form,
            startDate: this.form.startDate
                ? new Date(this.form.startDate).toISOString()
                : null,
            endDate: this.form.isCurrentlyStudying || !this.form.endDate
                ? null
                : new Date(this.form.endDate).toISOString()
        };

        const request = this.data.entry?.id
            ? this.profileService.updateEducation(this.data.entry.id, payload)
            : this.profileService.createEducation(payload);

        this.isLoading = true;
        request.subscribe({
            next: () => {
                this.isLoading = false;
                this.snackBar.open('Experiência educacional salva com sucesso.', 'Close', { duration: 3000 });
                this.dialogRef.close(true);
            },
            error: () => {
                this.isLoading = false;
                this.snackBar.open('Falha ao salvar. Por favor, tente novamente.', 'Close', { duration: 3000 });
            }
        });
    }
}