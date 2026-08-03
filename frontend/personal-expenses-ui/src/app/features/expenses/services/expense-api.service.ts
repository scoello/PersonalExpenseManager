import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Expense, SaveExpenseRequest } from '../models/expense.model';
import { API_BASE_URL } from '../../../core/config/api.config';

@Injectable({ providedIn: 'root' })
export class ExpenseApiService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${API_BASE_URL}/expenses`;

  list(): Observable<Expense[]> {
    return this.http.get<Expense[]>(this.endpoint);
  }

  create(expense: SaveExpenseRequest): Observable<Expense> {
    return this.http.post<Expense>(this.endpoint, expense);
  }

  update(id: string, expense: SaveExpenseRequest): Observable<void> {
    return this.http.put<void>(`${this.endpoint}/${id}`, expense);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}
