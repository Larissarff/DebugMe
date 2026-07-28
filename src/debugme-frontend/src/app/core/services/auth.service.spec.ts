import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { TokenResponse } from '../models/user.model';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  const mockTokenResponse: TokenResponse = {
    token: 'mock-jwt-token',
    refreshToken: 'mock-refresh-token',
    expiresAt: new Date(Date.now() + 3600000).toISOString(),
    user: {
      id: 'user-id-123',
      name: 'Test User',
      email: 'test@example.com',
      createdAt: '2026-01-01T00:00:00Z',
    },
  };

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [AuthService, provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login and store tokens', () => {
    service.login({ email: 'test@example.com', password: 'password123' }).subscribe((response) => {
      expect(response.token).toBe('mock-jwt-token');
      expect(response.user.email).toBe('test@example.com');
    });

    const req = httpMock.expectOne('http://localhost:5165/api/user/login');
    expect(req.request.method).toBe('POST');
    req.flush(mockTokenResponse);

    expect(service.getToken()).toBe('mock-jwt-token');
    expect(service.isLoggedIn()).toBeTruthy();
  });

  it('should register and store tokens', () => {
    service.register({ name: 'New User', email: 'new@example.com', password: 'password' }).subscribe((response) => {
      expect(response.token).toBe('mock-jwt-token');
    });

    const req = httpMock.expectOne('http://localhost:5165/api/user/create');
    expect(req.request.method).toBe('POST');
    req.flush(mockTokenResponse);
  });

  it('should return null for getCurrentUser when not logged in', () => {
    expect(service.getCurrentUser()).toBeNull();
  });

  it('should return user when logged in', () => {
    localStorage.setItem('debugme_token', 'mock-token');
    localStorage.setItem('debugme_user', JSON.stringify(mockTokenResponse));

    const user = service.getCurrentUser();
    expect(user).toBeTruthy();
    expect(user!.email).toBe('test@example.com');
  });

  it('should return false for isLoggedIn when no token', () => {
    expect(service.isLoggedIn()).toBeFalsy();
  });

  it('should clear storage on logout', () => {
    localStorage.setItem('debugme_token', 'mock-token');
    localStorage.setItem('debugme_user', JSON.stringify(mockTokenResponse));

    service.logout();

    expect(localStorage.getItem('debugme_token')).toBeNull();
    expect(localStorage.getItem('debugme_user')).toBeNull();
    expect(service.isLoggedIn()).toBeFalsy();
  });

  it('should detect expired token', () => {
    const expiredResponse = { ...mockTokenResponse, expiresAt: new Date(Date.now() - 1000).toISOString() };
    localStorage.setItem('debugme_token', 'mock-token');
    localStorage.setItem('debugme_user', JSON.stringify(expiredResponse));

    expect(service.isLoggedIn()).toBeFalsy();
  });
});
