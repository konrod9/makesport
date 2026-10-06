import type { AuthUser } from "@/entities/auth/types";
import { create } from "zustand";
import { createJSONStorage, persist } from "zustand/middleware";
import { useShallow } from "zustand/react/shallow";

export type AuthState = {
  accessToken: string | null;
  refreshToken: string | null;
  user: AuthUser | null;
};

type AuthActions = {
  setSession: (
    accessToken: string,
    refreshToken: string,
    user: AuthUser,
  ) => void;
  clearSession: () => void;
  setUser: (user: AuthUser) => void;
};

type AuthStore = AuthState & AuthActions;

const initialState: AuthState = {
  accessToken: null,
  refreshToken: null,
  user: null,
};

export const useAuthStore = create<AuthStore>()(
  persist(
    (set) => ({
      ...initialState,
      setSession: (accessToken, refreshToken, user) =>
        set(() => ({ accessToken, refreshToken, user })),
      clearSession: () => set(() => ({ ...initialState })),
      setUser: (user) => set(() => ({ user })),
    }),
    {
      name: "auth-session",
      storage: createJSONStorage(() => localStorage),
      partialize: (s) => ({
        accessToken: s.accessToken,
        refreshToken: s.refreshToken,
        user: s.user,
      }),
    },
  ),
);

export const useAuthSession = () =>
  useAuthStore(
    useShallow((s) => ({
      accessToken: s.accessToken,
      user: s.user,
      isAuthenticated: !!s.accessToken && !!s.user,
    })),
  );

export const getAccessToken = () => useAuthStore.getState().accessToken;
export const getRefreshToken = () => useAuthStore.getState().refreshToken;
export const applySession = (a: string, r: string, u: AuthUser) =>
  useAuthStore.getState().setSession(a, r, u);
export const dropSession = () => useAuthStore.getState().clearSession();
