import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MaterialModule } from '../../../../core/material.module';
import { ResumeService } from '../../../../core/services/resume.service';
import { PdfBuilderService } from '../../../../core/services/pdf-builder.service';
import { GeneratedResume, ResumeContent } from '../../../../core/models/profile.model';
import { ResumeParserService } from '../../../../core/services/resume-parser.service';

@Component({
  selector: 'app-resume-history',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './resume-history.html',
  styleUrl: './resume-history.css'
})
export class ResumeHistoryComponent implements OnInit {
  resumes: GeneratedResume[] = [];
  selectedResume: GeneratedResume | null = null;
  parsedContent: ResumeContent | null = null;
  isLoading = true;

  constructor(
    private resumeService: ResumeService,
    private pdfBuilder: PdfBuilderService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
    private resumeParser: ResumeParserService
  ) { }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading = true;
    this.cdr.markForCheck();
    this.resumeService.getHistory().subscribe({
      next: (data) => {
        this.resumes = data;
        this.isLoading = false;
        this.cdr.markForCheck();

      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  select(resume: GeneratedResume): void {
    const parsed = this.resumeParser.parse(resume);
    this.selectedResume = parsed;
    this.parsedContent = parsed.parsedContent ?? null;
    this.cdr.markForCheck();
  }

  back(): void {
    this.selectedResume = null;
    this.parsedContent = null;  
    this.cdr.markForCheck();
  }

  downloadPdf(resume: GeneratedResume): void {
    this.pdfBuilder.generate(resume);
    this.cdr.markForCheck();
  }

  delete(id: number): void {
    if (!confirm('Delete this resume from history?')) return;
    this.cdr.markForCheck();
    this.resumeService.delete(id).subscribe({
      next: () => {
        this.snackBar.open('Currículo deletado.', 'Close', { duration: 2000 });
        this.selectedResume = null;
        this.load();
        this.cdr.markForCheck();
      }
    });
  }
}