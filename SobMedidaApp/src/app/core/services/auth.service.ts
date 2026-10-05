import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { RegisterRequest, LoginRequest, AuthResponse } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://sob-medida-production.up.railway.app/auth';
  private tokenKey = 'sobmedida_token';

  constructor(private http: HttpClient, private router: Router) {}

  register(data: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data);
  }

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, data).pipe(
      tap(response => {
        localStorage.setItem(this.tokenKey, response.token);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiration = payload.exp * 1000;
      return Date.now() < expiration;
    } catch {
      return false;
    }
  }

  getFullName(): string {
    const token = this.getToken();
    if (!token) return '';

    try {
        const payload = JSON.parse(atob(token.split('.')[1]));

        return payload['fullName'] || '';
    } catch {
        return '';
    }
    }
    
    getMe(): Observable<any> {
        return this.http.get(`${this.apiUrl}/me`);
    }
}