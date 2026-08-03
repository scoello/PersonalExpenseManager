import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = () =>
  inject(AuthService).session() ? true : inject(Router).createUrlTree(['/login']);

export const guestGuard: CanActivateFn = () =>
  inject(AuthService).session() ? inject(Router).createUrlTree(['/expenses']) : true;

export const adminGuard: CanActivateFn = () =>
  inject(AuthService).session()?.role === 'Admin'
    ? true
    : inject(Router).createUrlTree(['/expenses']);
