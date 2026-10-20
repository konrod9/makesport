"use client";

import { useQuery } from "@tanstack/react-query";
import { useAuthStore } from "./auth-store";
import { authApi } from "@/entities/auth/api";

export const ME_KEY = ["me"];

export function useMe() {
  const hasToken = useAuthStore((s) => !!s.accessToken);
  return useQuery({
    queryKey: ME_KEY,
    queryFn: () => authApi.getMe(),
    enabled: hasToken,
    staleTime: 5 * 60 * 1000,
    retry: false,
  });
}
