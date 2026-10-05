import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ImportSummary, ImportedProfile } from '../models/import.model';

@Injectable({
  providedIn: 'root'
})
export class ImportService {
  private apiUrl = 'https://sob-medida-production.up.railway.app/import';

  constructor(private http: HttpClient) {}

  preview(file: File): Observable<ImportSummary> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ImportSummary>(`${this.apiUrl}/preview`, formData);
  }

  confirm(data: ImportedProfile): Observable<any> {
    return this.http.post(`${this.apiUrl}/confirm`, data);
  }
}