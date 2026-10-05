import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login';
import { RegisterComponent } from './features/auth/register/register';
import { DashboardComponent } from './features/dashboard/dashboard';
import { authGuard } from './core/guards/auth.guard';
import { ProfileComponent } from './features/profile/profile';
import { ResumeComponent } from './features/resume/resume';
import { ImportComponent } from './features/import/import';


export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [authGuard] },
  { path: 'resume', component: ResumeComponent, canActivate: [authGuard] },
  { path: 'import', component: ImportComponent, canActivate: [authGuard] }
];