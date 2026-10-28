import { CoordinatesDto } from "@/entities/venues/types";
import { useCallback, useEffect, useRef, useState } from "react";

export type GeolocationStatus =
  | "idle"
  | "loading"
  | "granted"
  | "denied"
  | "unavailable"
  | "error";

export type GeolocationErrorCode =
  | "unsupported"
  | "insecure_context"
  | "permission_denied"
  | "position_unavailable"
  | "timeout"
  | "unknown";

export type UserLocationState = {
  coordinates: CoordinatesDto | null;
  status: GeolocationStatus;
  errorCode: GeolocationErrorCode | null;
};

export type GeolocationRequestResult =
  | { coordinates: CoordinatesDto; errorCode: null }
  | { coordinates: null; errorCode: GeolocationErrorCode };

export type DistanceOriginResult =
  | {
      coordinates: CoordinatesDto;
      source: "geolocation" | "city";
      errorCode: null;
    }
  | {
      coordinates: null;
      source: null;
      errorCode: GeolocationErrorCode | "no_city";
    };

const GEOLOCATION_OPTIONS: PositionOptions = {
  enableHighAccuracy: false,
  timeout: 8000,
  maximumAge: 5 * 60 * 1000,
};

export function useUserLocation() {
  const [state, setState] = useState<UserLocationState>({
    coordinates: null,
    status: "idle",
    errorCode: null,
  });

  const mountedRef = useRef(true);
  const inFlightRef = useRef<Promise<GeolocationRequestResult> | null>(null);
  const coordinatesRef = useRef<CoordinatesDto | null>(null);

  useEffect(() => {
    mountedRef.current = true;
    return () => {
      mountedRef.current = false;
    };
  }, []);

  const requestLocation = useCallback((): Promise<GeolocationRequestResult> => {
    const cached = coordinatesRef.current;
    if (cached) {
      return Promise.resolve({ coordinates: cached, errorCode: null });
    }

    if (inFlightRef.current) {
      return inFlightRef.current;
    }

    if (typeof window === "undefined" || !("geolocation" in navigator)) {
      if (mountedRef.current) {
        setState({
          coordinates: null,
          status: "unavailable",
          errorCode: "unsupported",
        });
      }
      return Promise.resolve<GeolocationRequestResult>({
        coordinates: null,
        errorCode: "unsupported",
      });
    }

    if (!window.isSecureContext) {
      if (mountedRef.current) {
        setState({
          coordinates: null,
          status: "unavailable",
          errorCode: "insecure_context",
        });
      }
      return Promise.resolve<GeolocationRequestResult>({
        coordinates: null,
        errorCode: "insecure_context",
      });
    }

    if (mountedRef.current) {
      setState((prev) => ({
        ...prev,
        status: "loading",
        errorCode: null,
      }));
    }

    const promise = new Promise<GeolocationRequestResult>((resolve) => {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          const coordinates: CoordinatesDto = {
            latitude: position.coords.latitude,
            longitude: position.coords.longitude,
          };

          coordinatesRef.current = coordinates;

          if (mountedRef.current) {
            setState({
              coordinates,
              status: "granted",
              errorCode: null,
            });
          }

          resolve({ coordinates, errorCode: null });
        },
        (error) => {
          const errorCode: GeolocationErrorCode =
            error.code === error.PERMISSION_DENIED
              ? "permission_denied"
              : error.code === error.TIMEOUT
                ? "timeout"
                : error.code === error.POSITION_UNAVAILABLE
                  ? "position_unavailable"
                  : "unknown";

          if (mountedRef.current) {
            setState({
              coordinates: null,
              status: errorCode === "permission_denied" ? "denied" : "error",
              errorCode,
            });
          }

          resolve({ coordinates: null, errorCode });
        },
        GEOLOCATION_OPTIONS,
      );
    });

    const tracked = promise.finally(() => {
      inFlightRef.current = null;
    });

    inFlightRef.current = tracked;
    return tracked;
  }, []);

  return {
    coordinates: state.coordinates,
    status: state.status,
    errorCode: state.errorCode,
    requestLocation,
  };
}
