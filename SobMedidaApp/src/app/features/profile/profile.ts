import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MaterialModule } from '../../core/material.module';
import { PersonalInfoComponent } from './components/personal-info/personal-info';
import { ExperienceComponent } from './components/experience/experience';
import { EducationComponent } from './components/education/education';
import { SkillsComponent } from './components/skills/skills';
import { ProjectsComponent } from './components/projects/projects';
import { CertificationsComponent } from './components/certifications/certifications';
import { MatSnackBar } from '@angular/material/snack-bar';


@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MaterialModule,
    PersonalInfoComponent,
    ExperienceComponent,
    EducationComponent,
    SkillsComponent,
    ProjectsComponent,
    CertificationsComponent,
  ],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class ProfileComponent {
  constructor(private snackBar: MatSnackBar, private cdr: ChangeDetectorRef) { }

  onImportConcluido(): void {
    this.snackBar.open('Perfil atualizado! Navegue pelas abas para ver as informações importadas.', 'Fechar', { duration: 4000 });
    this.cdr.markForCheck();
  }
}

