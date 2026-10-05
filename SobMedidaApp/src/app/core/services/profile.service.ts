import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import {
  PersonalInfo,
  Experience,
  Education,
  Skill,
  Project,
  Certification
} from '../models/profile.model';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private apiUrl = 'https://sob-medida-production.up.railway.app/profile';

  private profileSubject = new BehaviorSubject<PersonalInfo | null>(null);

  public profile$ = this.profileSubject.asObservable();

  constructor(private http: HttpClient) { }

  // Personal Info
  getPersonalInfo(): Observable<PersonalInfo> {
    return this.http.get<PersonalInfo>(`${this.apiUrl}/personal-info`).pipe(
      tap(profile => this.profileSubject.next(profile))
    );
  }
  createPersonalInfo(data: PersonalInfo): Observable<any> {
    return this.http.post(`${this.apiUrl}/personal-info`, data).pipe(
      tap(() => this.profileSubject.next(data))
    );
  }
  updatePersonalInfo(data: PersonalInfo): Observable<any> {
    return this.http.put(`${this.apiUrl}/personal-info`, data).pipe(
      tap(() => {
        const currentProfile = this.profileSubject.value;
        this.profileSubject.next({ ...currentProfile, ...data });
      })
    );
  }

  // Experience
  getExperiences(): Observable<Experience[]> {
    return this.http.get<Experience[]>(`${this.apiUrl}/experiences`);
  }
  createExperience(data: Experience): Observable<any> {
    return this.http.post(`${this.apiUrl}/experiences`, data);
  }
  updateExperience(id: number, data: Experience): Observable<any> {
    return this.http.put(`${this.apiUrl}/experiences/${id}`, data);
  }
  deleteExperience(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/experiences/${id}`);
  }

  // Education
  getEducations(): Observable<Education[]> {
    return this.http.get<Education[]>(`${this.apiUrl}/educations`);
  }
  createEducation(data: Education): Observable<any> {
    return this.http.post(`${this.apiUrl}/educations`, data);
  }
  updateEducation(id: number, data: Education): Observable<any> {
    return this.http.put(`${this.apiUrl}/educations/${id}`, data);
  }
  deleteEducation(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/educations/${id}`);
  }

  // Skills
  getSkills(): Observable<Skill[]> {
    return this.http.get<Skill[]>(`${this.apiUrl}/skills`);
  }
  createSkill(data: Skill): Observable<any> {
    return this.http.post(`${this.apiUrl}/skills`, data);
  }
  updateSkill(id: number, data: Skill): Observable<any> {
    return this.http.put(`${this.apiUrl}/skills/${id}`, data);
  }
  deleteSkill(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/skills/${id}`);
  }

  // Projects
  getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(`${this.apiUrl}/projects`);
  }
  createProject(data: Project): Observable<any> {
    return this.http.post(`${this.apiUrl}/projects`, data);
  }
  updateProject(id: number, data: Project): Observable<any> {
    return this.http.put(`${this.apiUrl}/projects/${id}`, data);
  }
  deleteProject(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/projects/${id}`);
  }

  // Certifications
  getCertifications(): Observable<Certification[]> {
    return this.http.get<Certification[]>(`${this.apiUrl}/certifications`);
  }
  createCertification(data: Certification): Observable<any> {
    return this.http.post(`${this.apiUrl}/certifications`, data);
  }
  updateCertification(id: number, data: Certification): Observable<any> {
    return this.http.put(`${this.apiUrl}/certifications/${id}`, data);
  }
  deleteCertification(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/certifications/${id}`);
  }
}