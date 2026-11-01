
import { apiRequest } from "@/lib/api";

import type {
  AuthResponse,
  CurrentUser,
  LoginRequest,
  RegisterRequest,
  UserResponse,
} from "./types";

export function register(
  request: RegisterRequest,
): Promise<UserResponse> {
  return apiRequest<UserResponse>("/api/authentication/register", {
    method: "POST",
    body: request,
  });
}

export function login(
  request: LoginRequest,
): Promise<AuthResponse> {
  return apiRequest<AuthResponse>("/api/authentication/login", {
    method: "POST",
    body: request,
  });
}

export function refresh(): Promise<AuthResponse> {
  return apiRequest<AuthResponse>("/api/authentication/refresh", {
    method: "POST",
  });
}

export function logout(): Promise<void> {
  return apiRequest<void>("/api/authentication/logout", {
    method: "POST",
  });
}

export function getCurrentUser(): Promise<CurrentUser> {
  return apiRequest<CurrentUser>("/api/authentication/me");
}
