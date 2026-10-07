"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { dropSession, getRefreshToken } from "./auth-store";
import { authApi } from "@/entities/auth/api";

export function useLogout() {
  const queryClient = useQueryClient();
  const router = useRouter();
  const mutation = useMutation({
    mutationFn: async () => {
      const rt = getRefreshToken();
      if (rt) {
        try {
          await authApi.logout({ refreshToken: rt });
        } catch {}
      }
      dropSession();
      queryClient.clear();
    },
    onSettled: () => router.push("/auth/login"),
  });
  return {
    logout: mutation.mutate,
    isPending: mutation.isPending,
  };
}
