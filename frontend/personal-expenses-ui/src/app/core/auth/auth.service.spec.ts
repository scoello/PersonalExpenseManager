import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { Session } from './session.model';

describe('AuthService', () => {
  let service: AuthService;
  let http: HttpTestingController;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    localStorage.clear();
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [{ provide: Router, useValue: router }]
    });
    service = TestBed.inject(AuthService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    http.verify();
    localStorage.clear();
  });

  it('stores the authenticated session after login', () => {
    const session: Session = { token: 'token', username: 'admin', role: 'Admin' };

    service.login('admin', 'secret').subscribe(result => expect(result).toEqual(session));

    const request = http.expectOne('https://localhost:7177/api/auth/login');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ username: 'admin', password: 'secret' });
    request.flush(session);

    expect(service.session()).toEqual(session);
    expect(JSON.parse(localStorage.getItem('session') ?? '')).toEqual(session);
  });

  it('clears the session and redirects on logout', () => {
    localStorage.setItem('session', JSON.stringify({ token: 'token', username: 'admin', role: 'Admin' }));

    service.logout();

    expect(service.session()).toBeNull();
    expect(localStorage.getItem('session')).toBeNull();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });
});
