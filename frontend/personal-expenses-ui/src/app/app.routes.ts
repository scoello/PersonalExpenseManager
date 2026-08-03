import { Routes } from '@angular/router';
import { adminGuard, authGuard, guestGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  { path: 'login', canActivate: [guestGuard], loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'expenses', canActivate: [authGuard], loadComponent: () => import('./features/expenses/expenses-page/expenses-page.component').then(m => m.ExpensesPageComponent) },
  { path: 'users', canActivate: [authGuard, adminGuard], loadComponent: () => import('./features/users/users-page/users-page.component').then(m => m.UsersPageComponent) },
  { path: '', pathMatch: 'full', redirectTo: 'expenses' },
  { path: '**', redirectTo: 'expenses' }
];
