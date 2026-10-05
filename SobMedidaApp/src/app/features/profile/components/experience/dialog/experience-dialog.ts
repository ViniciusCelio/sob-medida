import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../../core/material.module';
import { Experience } from '../../../../../core/models/profile.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProfileService } from '../../../../../core/services/profile.service';

@Component({
    selector: 'app-experience-dialog',
    standalone: true,
    imports: [CommonModule, FormsModule, MaterialModule],
    templateUrl: './experience-dialog.html'
})
export class ExperienceDialogComponent {
    form: Experience;
    isLoading = false;

    constructor(
        public dialogRef: MatDialogRef<ExperienceDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: { entry?: Experience },
        private profileService: ProfileService,
        private snackBar: MatSnackBar
    ) {
        this.form = data.entry ? { ...data.entry } : {
            jobTitle: '', company: '', location: '',
            startDate: '', endDate: '', isCurrentJob: false, description: ''
        };
    }

    cancel(): void {
        this.dialogRef.close();
    }

    save(): void {
        if (this.form.startDate) {
            const startYear = new Date(this.form.startDate).getFullYear();
            if (startYear < 1900 || startYear > 2099) {
                this.snackBar.open('Por favor, selecione uma data de início válida.', 'Fechar', { duration: 3000 });
                return;
            }
        }

        if (!this.form.isCurrentJob && this.form.endDate) {
            const endYear = new Date(this.form.endDate).getFullYear();
            if (endYear < 1900 || endYear > 2099) {
                this.snackBar.open('Por favor, selecione uma data de fim válida.', 'Fechar', { duration: 3000 });
                return;
            }
        }

        if (this.form.startDate && this.form.endDate) {
            if (this.form.startDate > this.form.endDate) {
                this.snackBar.open('Data de Término não pode ser anterior a Data de Início', 'Fechar', { duration: 3000 });
                return;
            }
        }

        const payload: Experience = {
            ...this.form,
            startDate: this.form.startDate
                ? new Date(this.form.startDate).toISOString()
                : null,
            endDate: this.form.isCurrentJob || !this.form.endDate
                ? null
                : new Date(this.form.endDate).toISOString()
        };

        const request = this.data.entry?.id
            ? this.profileService.updateExperience(this.data.entry.id, payload)
            : this.profileService.createExperience(payload);

        this.isLoading = true;
        request.subscribe({
            next: () => {
                this.isLoading = false;
                this.snackBar.open('Experiência salva com sucesso.', 'Fechar', { duration: 3000 });
                this.dialogRef.close(true);
            },
            error: () => {
                this.isLoading = false;
                this.snackBar.open('Erro ao salvar. Tente novamente.', 'Fechar', { duration: 3000 });
            }
        });
    }
}