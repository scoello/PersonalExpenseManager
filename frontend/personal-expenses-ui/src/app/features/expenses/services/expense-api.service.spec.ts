import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ExpenseApiService } from './expense-api.service';

describe('ExpenseApiService', () => {
  let service: ExpenseApiService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    service = TestBed.inject(ExpenseApiService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('retrieves expenses', () => {
    const expenses = [{ id: '1', date: '2026-08-03', amount: 25, category: 'Food' }];
    service.list().subscribe(result => expect(result).toEqual(expenses));

    const request = http.expectOne('https://localhost:7177/api/expenses');
    expect(request.request.method).toBe('GET');
    request.flush(expenses);
  });

  it('creates an expense', () => {
    const draft = { date: '2026-08-03', amount: 25, category: 'Food' };
    service.create(draft).subscribe();

    const request = http.expectOne('https://localhost:7177/api/expenses');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(draft);
    request.flush({ id: '1', ...draft });
  });

  it('updates an expense by id', () => {
    const draft = { date: '2026-08-03', amount: 30, category: 'Food' };
    service.update('expense-id', draft).subscribe();

    const request = http.expectOne('https://localhost:7177/api/expenses/expense-id');
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(draft);
    request.flush(null);
  });

  it('deletes an expense by id', () => {
    service.delete('expense-id').subscribe();
    const request = http.expectOne('https://localhost:7177/api/expenses/expense-id');
    expect(request.request.method).toBe('DELETE');
    request.flush(null);
  });
});
