import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MaterialModule } from '../../core/material.module';
import { ResumeGeneratorComponent } from './components/resume-generator/resume-generator';
import { ResumeHistoryComponent } from './components/resume-history/resume-history';

@Component({
  selector: 'app-resume',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MaterialModule,
    ResumeGeneratorComponent,
    ResumeHistoryComponent
  ],
  templateUrl: './resume.html',
  styleUrl: './resume.css'
})
export class ResumeComponent {}