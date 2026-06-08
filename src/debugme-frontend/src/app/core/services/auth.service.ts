import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ApiService } from './api.service';
import { User, LoginRequest, CreateUserRequest } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly USER_KEY = 'debugme_user';

  constructor(private api: ApiService) {}

  login(credentials: LoginRequest): Observable<User> {
    return this.api
      .post<User>('/api/user/login', credentials)
      .pipe(tap((user) => this.setUser(user)));
  }

  register(data: CreateUserRequest): Observable<User> {
    return this.api
      .post<User>('/api/user/create', data)
      .pipe(tap((user) => this.setUser(user)));
  }

  logout(): void {
    localStorage.removeItem(this.USER_KEY);
  }

  getCurrentUser(): User | null {
    const stored = localStorage.getItem(this.USER_KEY);
    if (!stored) return null;
    try {
      return JSON.parse(stored) as User;
    } catch {
      return null;
    }
  }

  isLoggedIn(): boolean {
    return this.getCurrentUser() !== null;
  }

  private setUser(user: User): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }
}
