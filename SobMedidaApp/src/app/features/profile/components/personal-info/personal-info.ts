import { Component, OnInit, ChangeDetectorRef } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http'; 
import { MatSnackBar } from '@angular/material/snack-bar';
import { NgxMaskDirective, provideNgxMask } from 'ngx-mask';
import { MaterialModule } from '../../../../core/material.module';
import { ProfileService } from '../../../../core/services/profile.service';
import { PersonalInfo } from '../../../../core/models/profile.model';

@Component({
  selector: 'app-personal-info',
  standalone: true,
  imports: [CommonModule, FormsModule, MaterialModule, NgxMaskDirective],
  providers: [provideNgxMask()],
  templateUrl: './personal-info.html'
})
export class PersonalInfoComponent implements OnInit {
  form: PersonalInfo | null = null;
  isEditing = false; 
  isEditMode = false; 
  isLoading = false;
  isLoadingCep = false; 

  constructor(
    private profileService: ProfileService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
    private http: HttpClient 
  ) {}

  ngOnInit(): void {
    this.profileService.getPersonalInfo().subscribe({
      next: (data) => {
        this.form = { ...data };
        this.isEditing = true;
        this.isEditMode = false; 
        this.cdr.detectChanges(); 
      },
      error: () => { 
        this.isEditing = false; 
        this.isEditMode = true;
        this.form = {
          fullName: '',
          email: '',
          phone: '',
          address: {
            street: '',
            number: '',
            neighborhood: '',
            city: '',
            state: '',
            zipCode: ''
          },
          linkedInUrl: '',
          gitHubUrl: '',
          summary: ''
        };
        this.cdr.detectChanges(); 
      }
    });
  }

  enableEditMode(): void {
    this.isEditMode = true;
  }

  onSubmit(): void {
    if (!this.form) return; 

    this.isLoading = true;

    const request = this.isEditing
      ? this.profileService.updatePersonalInfo(this.form)
      : this.profileService.createPersonalInfo(this.form);

    request.subscribe({
      next: () => {
        this.isLoading = false;
        this.isEditing = true;
        this.isEditMode = false; 
        this.snackBar.open('Personal info saved successfully.', 'Close', { duration: 3000 });
        this.cdr.detectChanges(); 
      },
      error: () => {
        this.isLoading = false;
        this.snackBar.open('Failed to save. Please try again.', 'Close', { duration: 3000 });
        this.cdr.detectChanges();
      }
    });
  }

  clearAddressFields(): void {
    if (!this.form) return;
    this.form.address.street = '';
    this.form.address.neighborhood = '';
    this.form.address.city = '';
    this.form.address.state = '';
  }

  onCepChange(value: string): void {
    if (!this.form) return;

    let numericValue = value.replace(/\D/g, '');

    if (numericValue.length < 8 && this.form.address.state !== '') {
      this.clearAddressFields();
    }

    if (numericValue.length > 8) {
      numericValue = numericValue.substring(0, 8);
    }

    if (numericValue.length > 5) {
      numericValue = numericValue.replace(/^(\d{5})(\d)/, '$1-$2');
    }

    this.form.address.zipCode = numericValue;
  }

  searchCep(): void {
    if (!this.form || !this.form.address.zipCode) return;

    const cep = this.form.address.zipCode.replace(/\D/g, '');

    if (cep.length === 8) {
      this.isLoadingCep = true;
      this.http.get<any>(`https://viacep.com.br/ws/${cep}/json/`).subscribe({
        next: (data) => {
          this.isLoadingCep = false;
          
          if (!data.erro) {
            this.form!.address.street = data.logradouro;
            this.form!.address.neighborhood = data.bairro;
            this.form!.address.city = data.localidade;
            this.form!.address.state = data.uf;
          } else {
            this.clearAddressFields();
            this.snackBar.open('CEP not found.', 'Close', { duration: 3000 });
          }
          this.cdr.detectChanges();
        },
        error: () => {
          this.isLoadingCep = false;
          this.clearAddressFields();
          this.snackBar.open('Failed to fetch address details.', 'Close', { duration: 3000 });
          this.cdr.detectChanges();
        }
      });
    }
  }
}