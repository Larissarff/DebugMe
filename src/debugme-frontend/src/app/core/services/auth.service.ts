import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ApiService } from './api.service';
import { User, TokenResponse, LoginRequest, CreateUserRequest, RefreshTokenRequest } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'debugme_token';
  private readonly USER_KEY = 'debugme_user';

  constructor(private api: ApiService) {}

  login(credentials: LoginRequest): Observable<TokenResponse> {
    return this.api
      .post<TokenResponse>('/api/user/login', credentials)
      .pipe(tap((response) => this.storeTokens(response)));
  }

  register(data: CreateUserRequest): Observable<TokenResponse> {
    return this.api
      .post<TokenResponse>('/api/user/create', data)
      .pipe(tap((response) => this.storeTokens(response)));
  }

  refreshToken(): Observable<TokenResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      this.logout();
      return of(null as unknown as TokenResponse);
    }

    const body: RefreshTokenRequest = { refreshToken };
    return this.api
      .post<TokenResponse>('/api/user/refresh', body)
      .pipe(tap((response) => this.storeTokens(response)));
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    const stored = localStorage.getItem(this.USER_KEY);
    if (!stored) return null;
    try {
      const tokenResponse = JSON.parse(stored) as TokenResponse;
      return tokenResponse.refreshToken;
    } catch {
      return null;
    }
  }

  getCurrentUser(): User | null {
    const stored = localStorage.getItem(this.USER_KEY);
    if (!stored) return null;
    try {
      const tokenResponse = JSON.parse(stored) as TokenResponse;
      return tokenResponse.user;
    } catch {
      return null;
    }
  }

  isLoggedIn(): boolean {
    return this.getToken() !== null && !this.isTokenExpired();
  }

  isTokenExpired(): boolean {
    const stored = localStorage.getItem(this.USER_KEY);
    if (!stored) return true;
    try {
      const tokenResponse = JSON.parse(stored) as TokenResponse;
      const expiresAt = new Date(tokenResponse.expiresAt);
      return expiresAt <= new Date();
    } catch {
      return true;
    }
  }

  private storeTokens(response: TokenResponse): void {
    localStorage.setItem(this.TOKEN_KEY, response.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(response));
  }
}
