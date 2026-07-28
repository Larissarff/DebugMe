import { TestBed } from '@angular/core/testing';
import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideRouter, Router } from '@angular/router';
import { ApiService } from './api.service';

describe('ApiService', () => {
  let service: ApiService;
  let httpMock: HttpTestingController;
  let router: Router;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        ApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([])
      ],
    });

    service = TestBed.inject(ApiService);
    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should perform GET request', () => {
    const mockData = { id: 1, name: 'Test' };

    service.get<{ id: number; name: string }>('/api/test').subscribe((data) => {
      expect(data).toEqual(mockData);
    });

    const req = httpMock.expectOne('http://localhost:5165/api/test');
    expect(req.request.method).toBe('GET');
    req.flush(mockData);
  });

  it('should perform POST request', () => {
    const body = { name: 'Test' };
    const mockResponse = { id: 1, name: 'Test' };

    service.post<{ id: number; name: string }>('/api/test', body).subscribe((data) => {
      expect(data).toEqual(mockResponse);
    });

    const req = httpMock.expectOne('http://localhost:5165/api/test');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush(mockResponse);
  });

  it('should perform DELETE request', () => {
    service.delete<void>('/api/test/1').subscribe();

    const req = httpMock.expectOne('http://localhost:5165/api/test/1');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('should send auth header when token exists', () => {
    localStorage.setItem('debugme_token', 'test-token');

    service.get('/api/test').subscribe();

    const req = httpMock.expectOne('http://localhost:5165/api/test');
    expect(req.request.headers.get('Authorization')).toBe('Bearer test-token');
    req.flush({});
  });

  it('should not send auth header when no token', () => {
    service.get('/api/test').subscribe();

    const req = httpMock.expectOne('http://localhost:5165/api/test');
    expect(req.request.headers.get('Authorization')).toBeNull();
    req.flush({});
  });

  it('should extract error message from HTTP error', () => {
    service.get('/api/test').subscribe({
      error: (error) => {
        expect(error.message).toBe('Server error');
      },
    });

    const req = httpMock.expectOne('http://localhost:5165/api/test');
    req.flush({ message: 'Server error' }, { status: 500, statusText: 'Internal Server Error' });
  });

  it('should extract error message from 400 response', () => {
    service.post('/api/test', {}).subscribe({
      error: (error) => {
        expect(error.message).toBe('Dados inválidos');
      },
    });

    const req = httpMock.expectOne('http://localhost:5165/api/test');
    req.flush({ message: 'Dados inválidos' }, { status: 400, statusText: 'Bad Request' });
  });

  it('should attempt token refresh on 401 and retry once', () => {
    const refreshToken = 'old-refresh-token';
    const userData = {
      token: 'expired-token',
      refreshToken: refreshToken,
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      user: { id: '1', name: 'Test', email: 'test@test.com', createdAt: new Date().toISOString() }
    };
    localStorage.setItem('debugme_token', 'expired-token');
    localStorage.setItem('debugme_user', JSON.stringify(userData));

    const mockData = { id: 1, name: 'Test' };
    let receivedData: unknown = null;

    service.get<{ id: number; name: string }>('/api/test').subscribe({
      next: (data) => { receivedData = data; },
      error: () => { throw new Error('Should not error after successful refresh'); }
    });

    const req1 = httpMock.expectOne('http://localhost:5165/api/test');
    req1.flush({ message: 'Unauthorized' }, { status: 401, statusText: 'Unauthorized' });

    const refreshReq = httpMock.expectOne('http://localhost:5165/api/user/refresh');
    expect(refreshReq.request.body).toEqual({ refreshToken });
    const newTokens = {
      token: 'new-token',
      refreshToken: 'new-refresh-token',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      user: { id: '1', name: 'Test', email: 'test@test.com', createdAt: new Date().toISOString() }
    };
    refreshReq.flush(newTokens);

    const req2 = httpMock.expectOne('http://localhost:5165/api/test');
    expect(req2.request.headers.get('Authorization')).toBe('Bearer new-token');
    req2.flush(mockData);

    expect(receivedData).toEqual(mockData);
  });

  it('should redirect to login on 401 with no refresh token', () => {
    localStorage.setItem('debugme_token', 'expired-token');
    const navigateSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);

    service.get('/api/test').subscribe({
      error: (error) => {
        expect(error.message).toBe('Sessão expirada. Faça login novamente.');
      }
    });

    const req = httpMock.expectOne('http://localhost:5165/api/test');
    req.flush({ message: 'Unauthorized' }, { status: 401, statusText: 'Unauthorized' });

    expect(navigateSpy).toHaveBeenCalledWith(['/login']);
  });

  it('should redirect to login when refresh also fails', () => {
    const refreshToken = 'invalid-refresh-token';
    const userData = {
      token: 'expired-token',
      refreshToken: refreshToken,
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      user: { id: '1', name: 'Test', email: 'test@test.com', createdAt: new Date().toISOString() }
    };
    localStorage.setItem('debugme_token', 'expired-token');
    localStorage.setItem('debugme_user', JSON.stringify(userData));
    const navigateSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);

    service.get('/api/test').subscribe({
      error: (error) => {
        expect(error.message).toBe('Sessão expirada. Faça login novamente.');
      }
    });

    const req1 = httpMock.expectOne('http://localhost:5165/api/test');
    req1.flush({ message: 'Unauthorized' }, { status: 401, statusText: 'Unauthorized' });

    const refreshReq = httpMock.expectOne('http://localhost:5165/api/user/refresh');
    refreshReq.flush({ message: 'Invalid refresh token' }, { status: 401, statusText: 'Unauthorized' });

    expect(navigateSpy).toHaveBeenCalledWith(['/login']);
  });

  it('should not retry more than MAX_RETRY_ATTEMPTS times', () => {
    const refreshToken = 'some-refresh-token';
    const userData = {
      token: 'expired-token',
      refreshToken: refreshToken,
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      user: { id: '1', name: 'Test', email: 'test@test.com', createdAt: new Date().toISOString() }
    };
    localStorage.setItem('debugme_token', 'expired-token');
    localStorage.setItem('debugme_user', JSON.stringify(userData));
    const navigateSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);

    service.get('/api/test').subscribe({
      error: (error) => {
        expect(error.message).toBe('Sessão expirada. Faça login novamente.');
      }
    });

    const req1 = httpMock.expectOne('http://localhost:5165/api/test');
    req1.flush({ message: 'Unauthorized' }, { status: 401, statusText: 'Unauthorized' });

    const refreshReq1 = httpMock.expectOne('http://localhost:5165/api/user/refresh');
    const newTokens = {
      token: 'new-token',
      refreshToken: 'new-refresh-token',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      user: { id: '1', name: 'Test', email: 'test@test.com', createdAt: new Date().toISOString() }
    };
    refreshReq1.flush(newTokens);

    const req2 = httpMock.expectOne('http://localhost:5165/api/test');
    req2.flush({ message: 'Unauthorized' }, { status: 401, statusText: 'Unauthorized' });

    expect(navigateSpy).toHaveBeenCalledWith(['/login']);
  });
});
