import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, throwError, TimeoutError } from 'rxjs';
import { catchError, timeout, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl: string = environment.apiUrl;
  private readonly REQUEST_TIMEOUT = 30000; // 30 segundos

  constructor(private http: HttpClient) {
    console.log('[ApiService] Base URL:', this.baseUrl);
  }

  get<T>(endpoint: string, params?: HttpParams): Observable<T> {
    const url = `${this.baseUrl}${endpoint}`;
    console.log(`[ApiService] GET ${url}`);
    return this.http
      .get<T>(url, { params })
      .pipe(
        timeout(this.REQUEST_TIMEOUT),
        tap(response => console.log(`[ApiService] GET ${url} success:`, response)),
        catchError(this.handleError)
      );
  }

  post<T>(endpoint: string, body: unknown): Observable<T> {
    const url = `${this.baseUrl}${endpoint}`;
    console.log(`[ApiService] POST ${url}`, body);
    return this.http
      .post<T>(url, body)
      .pipe(
        timeout(this.REQUEST_TIMEOUT),
        tap(response => console.log(`[ApiService] POST ${url} success:`, response)),
        catchError(this.handleError)
      );
  }

  put<T>(endpoint: string, body: unknown): Observable<T> {
    const url = `${this.baseUrl}${endpoint}`;
    console.log(`[ApiService] PUT ${url}`, body);
    return this.http
      .put<T>(url, body)
      .pipe(
        timeout(this.REQUEST_TIMEOUT),
        tap(response => console.log(`[ApiService] PUT ${url} success:`, response)),
        catchError(this.handleError)
      );
  }

  delete<T>(endpoint: string): Observable<T> {
    const url = `${this.baseUrl}${endpoint}`;
    console.log(`[ApiService] DELETE ${url}`);
    return this.http
      .delete<T>(url)
      .pipe(
        timeout(this.REQUEST_TIMEOUT),
        tap(response => console.log(`[ApiService] DELETE ${url} success:`, response)),
        catchError(this.handleError)
      );
  }

  private handleError(error: HttpErrorResponse | TimeoutError): Observable<never> {
    let errorMessage = 'Ocorreu um erro inesperado.';

    if (error instanceof TimeoutError) {
      errorMessage = 'O servidor não respondeu a tempo. Verifique sua conexão.';
    } else if (error.error instanceof ErrorEvent) {
      errorMessage = `Erro de conexão: ${error.error.message}`;
    } else {
      errorMessage = error.error?.message || `Erro ${error.status}: ${error.statusText}`;
    }

    console.error('[ApiService] Error:', error);
    console.error('[ApiService] Error message:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
