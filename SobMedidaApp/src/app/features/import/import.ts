import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatExpansionModule } from '@angular/material/expansion';
import { MaterialModule } from '../../core/material.module';
import { ImportService } from '../../core/services/import.service';
import { ImportSummary, ImportedProfile } from '../../core/models/import.model';
import {
  PersonalInfo,
  Experience,
  Education,
  Skill,
  Project,
  Certification
} from '../../core/models/profile.model';

/**
 * Wraps a single extracted value so the review screen can track edits and
 * discards independently of the payload returned by the backend, without
 * mutating `summary` itself (which stays as the source of truth for counts).
 */
interface ReviewRow<T> {
  id: string;
  value: T;
  original: T;
  discarded: boolean;
  edited: boolean;
}

type CategoryKey =
  | 'personalInfo'
  | 'experiences'
  | 'educations'
  | 'skills'
  | 'projects'
  | 'certifications';

@Component({
  selector: 'app-import',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, MaterialModule, MatExpansionModule],
  templateUrl: './import.html',
  styleUrl: './import.css'
})
export class ImportComponent {
  private static nextRowId = 0;

  selectedFile: File | null = null;
  summary: ImportSummary | null = null;
  isLoadingPreview = false;
  isLoadingConfirm = false;
  step: 'upload' | 'preview' | 'done' = 'upload';

  // Editable working copies shown on the review screen. These are what get
  // sent to the backend on confirm — `summary.extractedData` is left untouched
  // so we always have the original extraction to diff against.
  personalInfoRow: ReviewRow<PersonalInfo> | null = null;
  personalInfoDiscarded = false;

  experienceRows: ReviewRow<Experience>[] = [];
  educationRows: ReviewRow<Education>[] = [];
  skillRows: ReviewRow<Skill>[] = [];
  projectRows: ReviewRow<Project>[] = [];
  certificationRows: ReviewRow<Certification>[] = [];

  // Tracks which category panels the user has actually opened, so we can show
  // an honest "reviewed" indicator instead of assuming they looked at everything.
  private reviewedCategories = new Set<CategoryKey>();

  constructor(
    private importService: ImportService,
    private snackBar: MatSnackBar,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.cdr.markForCheck();
    }
  }

  onPreview(): void {
    if (!this.selectedFile) return;

    this.isLoadingPreview = true;
    this.importService.preview(this.selectedFile).subscribe({
      next: (result) => {
        this.summary = result;
        this.buildReviewState(result.extractedData);
        this.step = 'preview';
        this.isLoadingPreview = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoadingPreview = false;
        this.snackBar.open(
          'Erro ao processar o arquivo. Verifique se é um PDF válido.',
          'Fechar',
          { duration: 4000 }
        );
        this.cdr.markForCheck();
      }
    });
  }

  onConfirm(): void {
    if (!this.summary) return;

    this.isLoadingConfirm = true;
    this.importService.confirm(this.buildPayload()).subscribe({
      next: () => {
        this.isLoadingConfirm = false;
        this.step = 'done';
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoadingConfirm = false;
        this.snackBar.open(
          'Erro ao salvar os dados. Tente novamente.',
          'Fechar',
          { duration: 4000 }
        );
        this.cdr.markForCheck();
      }
    });
  }

  onCancel(): void {
    this.selectedFile = null;
    this.summary = null;
    this.resetReviewState();
    this.step = 'upload';
    this.cdr.markForCheck();
  }

  goToProfile(): void {
    this.router.navigate(['/profile']);
  }

  // ---- Review state: build / reset ----------------------------------

  private buildReviewState(data: ImportedProfile): void {
    this.personalInfoRow = data.personalInfo ? this.toRow(data.personalInfo) : null;
    this.personalInfoDiscarded = false;

    this.experienceRows = this.toRows(data.experiences ?? []);
    this.educationRows = this.toRows(data.educations ?? []);
    this.skillRows = this.toRows(data.skills ?? []);
    this.projectRows = this.toRows(data.projects ?? []);
    this.certificationRows = this.toRows(data.certifications ?? []);
    this.reviewedCategories.clear();
  }

  private resetReviewState(): void {
    this.personalInfoRow = null;
    this.personalInfoDiscarded = false;
    this.experienceRows = [];
    this.educationRows = [];
    this.skillRows = [];
    this.projectRows = [];
    this.certificationRows = [];
    this.reviewedCategories.clear();
  }

  private toRow<T>(item: T): ReviewRow<T> {
    return {
      id: `row-${ImportComponent.nextRowId++}`,
      value: this.deepClone(item),
      original: this.deepClone(item),
      discarded: false,
      edited: false
    };
  }

  private toRows<T>(items: T[]): ReviewRow<T>[] {
    return items.map((item) => this.toRow(item));
  }

  private deepClone<T>(value: T): T {
    return JSON.parse(JSON.stringify(value)) as T;
  }

  // ---- Review state: user interactions -------------------------------

  /** Bind to (ngModelChange) on any editable field to refresh its "edited" badge. */
  markEdited<T>(row: ReviewRow<T>): void {
    row.edited = JSON.stringify(row.value) !== JSON.stringify(row.original);
  }

  toggleDiscard<T>(row: ReviewRow<T>): void {
    row.discarded = !row.discarded;
  }

  discardCategory(rows: ReviewRow<unknown>[]): void {
    rows.forEach((row) => (row.discarded = true));
  }

  restoreCategory(rows: ReviewRow<unknown>[]): void {
    rows.forEach((row) => (row.discarded = false));
  }

  togglePersonalInfo(): void {
    this.personalInfoDiscarded = !this.personalInfoDiscarded;
  }

  markCategoryReviewed(category: CategoryKey): void {
    this.reviewedCategories.add(category);
  }

  isCategoryReviewed(category: CategoryKey): boolean {
    return this.reviewedCategories.has(category);
  }

  isCategoryEdited(rows: ReviewRow<unknown>[]): boolean {
    return rows.some((row) => row.edited);
  }

  activeCount(rows: ReviewRow<unknown>[]): number {
    return rows.filter((row) => !row.discarded).length;
  }

  trackByRowId = (_: number, row: ReviewRow<unknown>): string => row.id;

  // ---- Payload --------------------------------------------------------

  private buildPayload(): ImportedProfile {
    return {
      personalInfo:
        this.personalInfoRow && !this.personalInfoDiscarded
          ? this.personalInfoRow.value
          : null,
      experiences: this.activeValues(this.experienceRows),
      educations: this.activeValues(this.educationRows),
      skills: this.activeValues(this.skillRows),
      projects: this.activeValues(this.projectRows),
      certifications: this.activeValues(this.certificationRows)
    };
  }

  private activeValues<T>(rows: ReviewRow<T>[]): T[] {
    return rows.filter((row) => !row.discarded).map((row) => row.value);
  }

  hasSomethingToImport(): boolean {
    if (!this.summary) return false;
    return (
      (this.personalInfoRow != null && !this.personalInfoDiscarded) ||
      this.activeCount(this.experienceRows) > 0 ||
      this.activeCount(this.educationRows) > 0 ||
      this.activeCount(this.skillRows) > 0 ||
      this.activeCount(this.projectRows) > 0 ||
      this.activeCount(this.certificationRows) > 0
    );
  }
}