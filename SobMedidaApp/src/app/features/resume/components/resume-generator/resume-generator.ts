import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MaterialModule } from '../../../../core/material.module';
import { ResumeService } from '../../../../core/services/resume.service';
import { PdfBuilderService } from '../../../../core/services/pdf-builder.service';
import { ResumeParserService } from '../../../../core/services/resume-parser.service';
import { GeneratedResume, ResumeContent } from '../../../../core/models/profile.model';

@Component({
  selector: 'app-resume-generator',
  standalone: true,
  imports: [CommonModule, FormsModule, MaterialModule],
  templateUrl: './resume-generator.html',
  styleUrl: './resume-generator.css'
})
export class ResumeGeneratorComponent {
  jobTitle = '';
  jobDescription = '';
  generatedResume: GeneratedResume | null = null;
  parsedContent: ResumeContent | null = null;
  isLoading = false;

  constructor(
    private resumeService: ResumeService,
    private pdfBuilder: PdfBuilderService,
    private resumeParser: ResumeParserService,
    private snackBar: MatSnackBar,
    private cdr : ChangeDetectorRef
  ) {}

  onGenerate(): void {
    if (!this.jobTitle.trim() || !this.jobDescription.trim()) {
      this.snackBar.open('Por favor, preencha o título e descrição da vaga.', 'Fechar', { duration: 3000 });
      this.cdr.markForCheck();
      return;
    }

    this.isLoading = true;
    this.generatedResume = null;
    this.parsedContent = null;

    this.resumeService.generate({
      jobTitle: this.jobTitle,
      jobDescription: this.jobDescription
    }).subscribe({
      next: (result) => {
        this.isLoading = false;
        const parsed = this.resumeParser.parse(result);
        this.generatedResume = parsed;
        this.parsedContent = parsed.parsedContent ?? null;
        this.snackBar.open('Currículo gerado com sucesso!', 'Fechar', { duration: 3000 });
        this.cdr.markForCheck();
      },
      error: (error: HttpErrorResponse) => {
        this.isLoading = false;
        this.snackBar.open(this.getErrorMessage(error), 'Fechar', { duration: 5000 });
        this.cdr.markForCheck();
      }
    });
  }

  private getErrorMessage(error: HttpErrorResponse): string {
    const message = error.error?.message;

    if (typeof message === 'string' && message.trim()) {
      return message;
    }

    if (error.status === 0) {
      return 'Não foi possível conectar ao servidor. Verifique sua conexão e tente novamente.';
    }

    return 'Falha ao gerar o currículo. Por favor, tente novamente.';
  }

  downloadPdf(): void {
    if (!this.generatedResume) return;
    this.pdfBuilder.generate(this.generatedResume);
    this.cdr.markForCheck();
  }

  reset(): void {
    this.jobTitle = '';
    this.jobDescription = '';
    this.generatedResume = null;
    this.parsedContent = null;
    this.cdr.markForCheck();
  }
}