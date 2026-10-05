import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../../../core/material.module';
import { Certification } from '../../../../../core/models/profile.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProfileService } from '../../../../../core/services/profile.service';

@Component({
    selector: 'app-certification-dialog',
    standalone: true,
    imports: [CommonModule, FormsModule, MaterialModule],
    templateUrl: './certification-dialog.html'
})
export class CertificationDialogComponent {
    form: Certification;
    isLoading = false;

    constructor(
        public dialogRef: MatDialogRef<CertificationDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: { entry?: Certification },
        private profileService: ProfileService,
        private snackBar: MatSnackBar
    ) {
        this.form = data.entry ? { ...data.entry } : {
            name: '', issuingOrganization: '', issueDate: '',
            expirationDate: '', doesNotExpire: false, credentialUrl: ''
        };
    }

    onDoesNotExpireChange(): void {
        if (this.form.doesNotExpire) {
            this.form.expirationDate = '';
        }
    }

    cancel(): void {
        this.dialogRef.close();
    }

    save(): void {
        if (this.form.issueDate) {
            const issueYear = new Date(this.form.issueDate).getFullYear();
            if (issueYear < 1900 || issueYear > 2099) {
                this.snackBar.open('Please enter a valid Issue Date.', 'Close', { duration: 3000 });
                return;
            }
        }

        if (!this.form.doesNotExpire && this.form.expirationDate) {
            const expirationYear = new Date(this.form.expirationDate).getFullYear();
            if (expirationYear < 1900 || expirationYear > 2099) {
                this.snackBar.open('Please enter a valid Expiration Date.', 'Close', { duration: 3000 });
                return;
            }
        }

        if (this.form.issueDate && this.form.expirationDate) {
            if (this.form.issueDate > this.form.expirationDate) {
                this.snackBar.open('Expiration Date cannot be earlier than Issue Date.', 'Close', { duration: 3000 });
                return;
            }
        }

        const payload: Certification = {
            ...this.form,
            issueDate: this.form.issueDate
                ? new Date(this.form.issueDate).toISOString()
                : null,
            expirationDate: this.form.expirationDate && !this.form.doesNotExpire
                ? new Date(this.form.expirationDate).toISOString()
                : null
        };

        const request = this.data.entry?.id
            ? this.profileService.updateCertification(this.data.entry.id, payload)
            : this.profileService.createCertification(payload);

        this.isLoading = true;
        request.subscribe({
            next: () => {
                this.isLoading = false;
                this.snackBar.open('Certification saved successfully.', 'Close', { duration: 3000 });
                this.dialogRef.close(true);
            },
            error: () => {
                this.isLoading = false;
                this.snackBar.open('Failed to save. Please try again.', 'Close', { duration: 3000 });
            }
        });
    }
}