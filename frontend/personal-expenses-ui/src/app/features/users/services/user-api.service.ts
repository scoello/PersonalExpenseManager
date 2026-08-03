import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateUserRequest, User } from '../models/user.model';
import { API_BASE_URL } from '../../../core/config/api.config';

@Injectable({ providedIn: 'root' })
export class UserApiService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${API_BASE_URL}/users`;

  list(): Observable<User[]> {
    return this.http.get<User[]>(this.endpoint);
  }

  create(request: CreateUserRequest): Observable<User> {
    return this.http.post<User>(this.endpoint, request);
  }
}
