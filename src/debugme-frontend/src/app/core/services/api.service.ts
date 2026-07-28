import { Injectable, Injector } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, throwError, TimeoutError } from 'rxjs';
import { catchError, timeout, tap, switchMap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl: string = environment.apiUrl;
  private readonly REQUEST_TIMEOUT = 30000;
  private readonly MAX_RETRY_ATTEMPTS = 1;

  constructor(
    private http: HttpClient,
    private injector: Injector
  ) {
    console.log('[ApiService] Base URL:', this.baseUrl);
  }

  private get router(): Router {
    return this.injector.get(Router);
  }

  get<T>(endpoint: string, params?: HttpParams, retryCount: number = 0): Observable<T> {
    const url = `${this.baseUrl}${endpoint}`;
    console.log(`[ApiService] GET ${url}`);
    return this.http
      .get<T>(url, { params, headers: this.getAuthHeaders() })
      .pipe(
        timeout(this.REQUEST_TIMEOUT),
        tap(response => console.log(`[ApiService] GET ${url} success:`, response)),
        catchError(error => this.handleAuthError(error, () => this.get<T>(endpoint, params, retryCount + 1), retryCount))
      );
  }

  post<T>(endpoint: string, body: unknown, retryCount: number = 0): Observable<T> {
    const url = `${this.baseUrl}${endpoint}`;
    console.log(`[ApiService] POST ${url}`, body);
    return this.http
      .post<T>(url, body, { headers: this.getAuthHeaders() })
      .pipe(
        timeout(this.REQUEST_TIMEOUT),
        tap(response => console.log(`[ApiService] POST ${url} success:`, response)),
        catchError(error => this.handleAuthError(error, () => this.post<T>(endpoint, body, retryCount + 1), retryCount))
      );
  }

  put<T>(endpoint: string, body: unknown, retryCount: number = 0): Observable<T> {
    const url = `${this.baseUrl}${endpoint}`;
    console.log(`[ApiService] PUT ${url}`, body);
    return this.http
      .put<T>(url, body, { headers: this.getAuthHeaders() })
      .pipe(
        timeout(this.REQUEST_TIMEOUT),
        tap(response => console.log(`[ApiService] PUT ${url} success:`, response)),
        catchError(error => this.handleAuthError(error, () => this.put<T>(endpoint, body, retryCount + 1), retryCount))
      );
  }

  delete<T>(endpoint: string, retryCount: number = 0): Observable<T> {
    const url = `${this.baseUrl}${endpoint}`;
    console.log(`[ApiService] DELETE ${url}`);
    return this.http
      .delete<T>(url, { headers: this.getAuthHeaders() })
      .pipe(
        timeout(this.REQUEST_TIMEOUT),
        tap(response => console.log(`[ApiService] DELETE ${url} success:`, response)),
        catchError(error => this.handleAuthError(error, () => this.delete<T>(endpoint, retryCount + 1), retryCount))
      );
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('debugme_token');
    if (token) {
      return new HttpHeaders({
        Authorization: `Bearer ${token}`
      });
    }
    return new HttpHeaders();
  }

  private handleAuthError(
    error: HttpErrorResponse | TimeoutError,
    retryFn: () => Observable<unknown>,
    retryCount: number
  ): Observable<never> {
    if (error instanceof HttpErrorResponse && error.status === 401) {
      if (retryCount >= this.MAX_RETRY_ATTEMPTS) {
        this.clearAuthAndRedirect();
        return throwError(() => new Error('Sessão expirada. Faça login novamente.'));
      }

      const refreshToken = this.getStoredRefreshToken();
      if (refreshToken) {
        const body = { refreshToken };
        return this.http
          .post<TokenResponse>(`${this.baseUrl}/api/user/refresh`, body)
          .pipe(
            switchMap((response) => {
              localStorage.setItem('debugme_token', response.token);
              localStorage.setItem('debugme_user', JSON.stringify(response));
              return retryFn();
            }),
            catchError(() => {
              this.clearAuthAndRedirect();
              return throwError(() => new Error('Sessão expirada. Faça login novamente.'));
            })
          ) as Observable<never>;
      }

      this.clearAuthAndRedirect();
      return throwError(() => new Error('Sessão expirada. Faça login novamente.'));
    }

    return throwError(() => this.extractErrorMessage(error));
  }

  private clearAuthAndRedirect(): void {
    this.clearAuth();
    this.router.navigate(['/login']);
  }

  private extractErrorMessage(error: HttpErrorResponse | TimeoutError): Error {
    if (error instanceof TimeoutError) {
      return new Error('O servidor não respondeu a tempo. Verifique sua conexão.');
    }

    if (error.error instanceof ErrorEvent) {
      return new Error(`Erro de conexão: ${error.error.message}`);
    }

    if (error.error && typeof error.error === 'object' && error.error.message) {
      return new Error(error.error.message);
    }

    if (error.status === 0) {
      return new Error('Não foi possível conectar ao servidor. Verifique se o backend está rodando.');
    }

    if (error.status === 500) {
      return new Error('Erro interno do servidor. Tente novamente mais tarde.');
    }

    return new Error(`Erro ${error.status}: ${error.statusText || 'requisição falhou'}`);
  }

  private getStoredRefreshToken(): string | null {
    const stored = localStorage.getItem('debugme_user');
    if (!stored) return null;
    try {
      const tokenResponse = JSON.parse(stored) as TokenResponse;
      return tokenResponse.refreshToken;
    } catch {
      return null;
    }
  }

  private clearAuth(): void {
    localStorage.removeItem('debugme_token');
    localStorage.removeItem('debugme_user');
  }
}

interface TokenResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: { id: string; name: string; email: string; createdAt: string };
}
