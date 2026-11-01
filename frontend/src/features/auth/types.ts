
export type RegisterRequest = {
  email: string;
  password: string;
  confirmPassword: string;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type UserResponse = {
  id: string;
  email: string;
};

export type AuthResponse = {
  userId: string;
  email: string;
  accessToken: string;
  expiresAt: string;
};

export type CurrentUser = {
  userId: string;
  email: string;
};
