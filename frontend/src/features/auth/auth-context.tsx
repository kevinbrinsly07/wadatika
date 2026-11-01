"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";

import {
  getCurrentUser,
  login as loginService,
  logout as logoutService,
  refresh,
  register as registerService,
} from "./auth-service";

import {
  clearAccessToken,
  setAccessToken,
} from "./token-store";

import type {
  CurrentUser,
  LoginRequest,
  RegisterRequest,
  UserResponse,
} from "./types";

type AuthStatus = "loading" | "authenticated" | "unauthenticated";

type AuthContextValue = {
  user: CurrentUser | null;
  status: AuthStatus;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<UserResponse>;
  logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

// Prevent duplicate refresh requests during React Strict Mode initialization.
let sessionInitialization: Promise<CurrentUser | null> | null = null;

function initializeSession(): Promise<CurrentUser | null> {
  if (!sessionInitialization) {
    sessionInitialization = (async () => {
      try {
        const response = await refresh();

        setAccessToken(response.accessToken);

        return await getCurrentUser();
      } catch {
        clearAccessToken();
        return null;
      } finally {
        sessionInitialization = null;
      }
    })();
  }

  return sessionInitialization;
}

export function AuthProvider({
  children,
}: {
  children: ReactNode;
}) {
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [status, setStatus] = useState<AuthStatus>("loading");

  useEffect(() => {
    let cancelled = false;

    void initializeSession().then((currentUser) => {
      if (cancelled) {
        return;
      }

      setUser(currentUser);
      setStatus(
        currentUser ? "authenticated" : "unauthenticated",
      );
    });

    return () => {
      cancelled = true;
    };
  }, []);

  const login = useCallback(async (request: LoginRequest) => {
    const response = await loginService(request);

    setAccessToken(response.accessToken);

    setUser({
      userId: response.userId,
      email: response.email,
    });

    setStatus("authenticated");
  }, []);

  const register = useCallback(
    async (request: RegisterRequest) => {
      return registerService(request);
    },
    [],
  );

  const logout = useCallback(async () => {
    try {
      await logoutService();
    } finally {
      clearAccessToken();
      setUser(null);
      setStatus("unauthenticated");
    }
  }, []);

  return (
    <AuthContext.Provider
      value={{
        user,
        status,
        login,
        register,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error(
      "useAuth must be used within an AuthProvider.",
    );
  }

  return context;
}