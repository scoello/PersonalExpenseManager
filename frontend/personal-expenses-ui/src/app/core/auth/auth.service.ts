import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { Session } from './session.model';
import { API_BASE_URL } from '../config/api.config';

const SESSION_KEY = 'session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly sessionState = signal<Session | null>(this.restoreSession());

  readonly session = this.sessionState.asReadonly();

  login(username: string, password: string): Observable<Session> {
    return this.http.post<Session>(`${API_BASE_URL}/auth/login`, { username, password }).pipe(
      tap(session => this.persistSession(session))
    );
  }

  logout(): void {
    this.persistSession(null);
    void this.router.navigate(['/login']);
  }

  private persistSession(session: Session | null): void {
    this.sessionState.set(session);
    session
      ? localStorage.setItem(SESSION_KEY, JSON.stringify(session))
      : localStorage.removeItem(SESSION_KEY);
  }

  private restoreSession(): Session | null {
    const storedSession = localStorage.getItem(SESSION_KEY);
    if (!storedSession) return null;
    try {
      return JSON.parse(storedSession) as Session;
    } catch {
      localStorage.removeItem(SESSION_KEY);
      return null;
    }
  }
}
