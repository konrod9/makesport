import axios from "axios";
import type { InternalAxiosRequestConfig } from "axios";
import { Envelope } from "./envelope";
import { EnvelopeError } from "./errors";
import type { AuthResponse } from "@/entities/auth/api";
import {
  getAccessToken,
  getRefreshToken,
  applySession,
  dropSession,
} from "@/features/auth/model/auth-store";

const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost/api";

export const apiClient = axios.create({
  baseURL: BASE_URL,
  headers: { "Content-Type": "application/json" },
});

const AUTH_SKIP_URLS = ["/auth/login", "/auth/register", "/auth/refresh"];

apiClient.interceptors.request.use((config) => {
  if (AUTH_SKIP_URLS.some((u) => config.url?.includes(u))) return config;

  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

const rawRefresh = (refreshToken: string) =>
  axios.post<Envelope<AuthResponse>>(
    `${BASE_URL}/auth/refresh`,
    { refreshToken },
    { headers: { "Content-Type": "application/json" } },
  );

let refreshPromise: Promise<string | null> | null = null;

const redirectToLogin = () => {
  if (typeof window === "undefined") return;
  const returnUrl = encodeURIComponent(
    window.location.pathname + window.location.search,
  );
  window.location.href = `/auth/login?returnUrl=${returnUrl}`;
};

type RetryableConfig = InternalAxiosRequestConfig & { _retry?: boolean };

const doRefreshAndRetry = async (
  config: RetryableConfig,
  onRefreshFailed: () => unknown,
): Promise<unknown> => {
  config._retry = true;

  const storedRefresh = getRefreshToken();
  if (!storedRefresh) {
    dropSession();
    redirectToLogin();
    return onRefreshFailed();
  }

  refreshPromise ??= rawRefresh(storedRefresh)
    .then((r) => {
      const result = r.data.result;
      if (!result?.accessToken || !result?.refreshToken || !result?.user) {
        dropSession();
        redirectToLogin();
        return null;
      }
      applySession(result.accessToken, result.refreshToken, result.user);
      return result.accessToken;
    })
    .catch(() => {
      dropSession();
      redirectToLogin();
      return null;
    })
    .finally(() => {
      refreshPromise = null;
    });

  const newAccess = await refreshPromise;
  if (!newAccess) return onRefreshFailed();
  config.headers.Authorization = `Bearer ${newAccess}`;
  return apiClient(config);
};

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (axios.isAxiosError(error)) {
      const config = error.config as RetryableConfig | undefined;
      const url: string = config?.url ?? "";
      const isSkip = AUTH_SKIP_URLS.some((u) => url.includes(u));
      const canRetry =
        !!config && !isSkip && !config._retry && error.response?.status === 401;

      const envelope = error.response?.data as Envelope | undefined;

      if (envelope?.isError && envelope.error) {
        const apiError = envelope.error;
        if (canRetry && config) {
          return doRefreshAndRetry(config, () => {
            throw new EnvelopeError(apiError);
          });
        }
        throw new EnvelopeError(apiError);
      }

      if (canRetry && config) {
        return doRefreshAndRetry(config, () => Promise.reject(error));
      }
    }

    return Promise.reject(error);
  },
);
