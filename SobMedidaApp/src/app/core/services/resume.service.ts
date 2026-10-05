import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GenerateResumeRequest, GeneratedResume } from '../models/profile.model';

@Injectable({
  providedIn: 'root'
})
export class ResumeService {
  private apiUrl = 'https://sob-medida-production.up.railway.app/resume';

  constructor(private http: HttpClient) {}

  generate(data: GenerateResumeRequest): Observable<GeneratedResume> {
    return this.http.post<GeneratedResume>(`${this.apiUrl}/generate`, data);
  }

  getHistory(): Observable<GeneratedResume[]> {
    return this.http.get<GeneratedResume[]>(`${this.apiUrl}/history`);
  }

  getById(id: number): Observable<GeneratedResume> {
    return this.http.get<GeneratedResume>(`${this.apiUrl}/history/${id}`);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/history/${id}`);
  }
}