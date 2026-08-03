import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { UserApiService } from './user-api.service';

describe('UserApiService', () => {
  let service: UserApiService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    service = TestBed.inject(UserApiService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('retrieves users', () => {
    const users = [{ id: '1', username: 'admin', role: 'Admin' }];
    service.list().subscribe(result => expect(result).toEqual(users));
    const request = http.expectOne('https://localhost:7177/api/users');
    expect(request.request.method).toBe('GET');
    request.flush(users);
  });

  it('creates a user', () => {
    const draft = { username: 'new-user', password: 'Password1!' };
    service.create(draft).subscribe();
    const request = http.expectOne('https://localhost:7177/api/users');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(draft);
    request.flush({ id: '2', username: draft.username, role: 'User' });
  });
});
