export interface User {
  id: string;
  name: string;
  email: string;
  createdAt: string;
}

export interface TokenResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface CreateUserRequest {
  name: string;
  email: string;
  password: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}
