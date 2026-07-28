import { TestBed } from '@angular/core/testing';
import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { ApiService } from './api.service';

describe('ApiService', () => {
  let service: ApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [ApiService, provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(ApiService);
    httpMock = TestBed.inject(HttpTestingController);
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
});
