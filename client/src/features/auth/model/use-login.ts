"use client";

import { authApi, LoginRequest } from "@/entities/auth/api";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { applySession } from "./auth-store";
import { EnvelopeError } from "@/shared/api/errors";

export function useLogin() {
  const queryClient = useQueryClient();
  const mutation = useMutation({
    mutationFn: (req: LoginRequest) => authApi.login(req),
    onSuccess: (result) => {
      if (!result) return;
      applySession(result.accessToken, result.refreshToken, result.user);
      queryClient.setQueryData(["me"], result.user);
    },
  });

  return {
    login: mutation.mutate,
    loginAsync: mutation.mutateAsync,
    isPending: mutation.isPending,
    isError: mutation.isError,
    error: mutation.error instanceof EnvelopeError ? mutation.error : undefined,
  };
}
