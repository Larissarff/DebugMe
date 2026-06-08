import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}

  get<T>(endpoint: string, params?: HttpParams): Observable<T> {
    return this.http
      .get<T>(`${this.baseUrl}${endpoint}`, { params })
      .pipe(catchError(this.handleError));
  }

  post<T>(endpoint: string, body: unknown): Observable<T> {
    return this.http
      .post<T>(`${this.baseUrl}${endpoint}`, body)
      .pipe(catchError(this.handleError));
  }

  put<T>(endpoint: string, body: unknown): Observable<T> {
    return this.http
      .put<T>(`${this.baseUrl}${endpoint}`, body)
      .pipe(catchError(this.handleError));
  }

  delete<T>(endpoint: string): Observable<T> {
    return this.http
      .delete<T>(`${this.baseUrl}${endpoint}`)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'Ocorreu um erro inesperado.';

    if (error.error instanceof ErrorEvent) {
      // Erro do lado do cliente (rede, timeout)
      errorMessage = `Erro de conexão: ${error.error.message}`;
    } else {
      // Erro do lado do servidor
      errorMessage = error.error?.message || `Erro ${error.status}: ${error.statusText}`;
    }

    console.error('[ApiService] Error:', error);
    return throwError(() => new Error(errorMessage));
  }
}
