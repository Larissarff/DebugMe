import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError, TimeoutError } from 'rxjs';
import { catchError, timeout, tap, switchMap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl: string = environment.apiUrl;
  private readonly REQUEST_TIMEOUT = 30000;

  constructor(private http: HttpClient) {
    console.log('[ApiService] Base URL:', this.baseUrl);
  }

  get<T>(endpoint: string, params?: HttpParams): Observable<T> {
    const url = `${this.baseUrl}${endpoint}`;
    console.log(`[ApiService] GET ${url}`);
    return this.http
      .get<T>(url, { params, headers: this.getAuthHeaders() })
      .pipe(
        timeout(this.REQUEST_TIMEOUT),
        tap(response => console.log(`[ApiService] GET ${url} success:`, response)),
        catchError(error => this.handleAuthError(error, () => this.get<T>(endpoint, params)))
      );
  }

  post<T>(endpoint: string, body: unknown): Observable<T> {
    const url = `${this.baseUrl}${endpoint}`;
    console.log(`[ApiService] POST ${url}`, body);
    return this.http
      .post<T>(url, body, { headers: this.getAuthHeaders() })
      .pipe(
        timeout(this.REQUEST_TIMEOUT),
        tap(response => console.log(`[ApiService] POST ${url} success:`, response)),
        catchError(error => this.handleAuthError(error, () => this.post<T>(endpoint, body)))
      );
  }

  put<T>(endpoint: string, body: unknown): Observable<T> {
    const url = `${this.baseUrl}${endpoint}`;
    console.log(`[ApiService] PUT ${url}`, body);
    return this.http
      .put<T>(url, body, { headers: this.getAuthHeaders() })
      .pipe(
        timeout(this.REQUEST_TIMEOUT),
        tap(response => console.log(`[ApiService] PUT ${url} success:`, response)),
        catchError(error => this.handleAuthError(error, () => this.put<T>(endpoint, body)))
      );
  }

  delete<T>(endpoint: string): Observable<T> {
    const url = `${this.baseUrl}${endpoint}`;
    console.log(`[ApiService] DELETE ${url}`);
    return this.http
      .delete<T>(url, { headers: this.getAuthHeaders() })
      .pipe(
        timeout(this.REQUEST_TIMEOUT),
        tap(response => console.log(`[ApiService] DELETE ${url} success:`, response)),
        catchError(error => this.handleAuthError(error, () => this.delete<T>(endpoint)))
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
    retryFn: () => Observable<unknown>
  ): Observable<never> {
    if (error instanceof HttpErrorResponse && error.status === 401) {
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
              this.clearAuth();
              window.location.href = '/login';
              return throwError(() => new Error('Sessão expirada. Faça login novamente.'));
            })
          ) as Observable<never>;
      }
      this.clearAuth();
      window.location.href = '/login';
      return throwError(() => new Error('Sessão expirada. Faça login novamente.'));
    }

    let errorMessage = 'Ocorreu um erro inesperado.';
    if (error instanceof TimeoutError) {
      errorMessage = 'O servidor não respondeu a tempo. Verifique sua conexão.';
    } else if (error.error instanceof ErrorEvent) {
      errorMessage = `Erro de conexão: ${error.error.message}`;
    } else {
      errorMessage = error.error?.message || `Erro ${error.status}: ${error.statusText}`;
    }

    console.error('[ApiService] Error:', error);
    return throwError(() => new Error(errorMessage));
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
