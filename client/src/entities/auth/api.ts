import { apiClient } from "@/shared/api/axios-instance";
import { Envelope } from "@/shared/api/envelope";
import { AuthUser } from "./types";

export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
};

export type RefreshRequest = {
  refreshToken: string;
};

export type LogoutRequest = {
  refreshToken: string;
};

export type TokenPair = {
  accessToken: string;
  refreshToken: string;
};

export type AuthResponse = {
  user: AuthUser;
  accessToken: string;
  refreshToken: string;
};

export const authApi = {
  register: async (request: RegisterRequest) => {
    const response = await apiClient.post<Envelope<AuthResponse>>(
      "/auth/register",
      request,
    );

    return response.data.result;
  },

  login: async (request: LoginRequest) => {
    const response = await apiClient.post<Envelope<AuthResponse>>(
      "/auth/login",
      request,
    );

    return response.data.result;
  },

  refresh: async (request: RefreshRequest) => {
    const response = await apiClient.post<Envelope<AuthResponse>>(
      "/auth/refresh",
      request,
    );

    return response.data.result;
  },

  logout: async (request: LogoutRequest) => {
    const response = await apiClient.post<Envelope>("/auth/logout", request);

    return response.data.result;
  },

  getMe: async () => {
    const response = await apiClient.get<Envelope<AuthUser>>("/auth/me");

    return response.data.result;
  },
};
