"use client";

import { FiltersSidebar } from "@/shared/components/filters-sidebar";
import { VenueCard } from "@/shared/components/venue-card";
import { VenueDetailsDialog } from "@/shared/components/venue-details-dialog";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useVenuesList } from "./model/use-venues-list";
import { Venue } from "@/entities/venues/types";
import { Spinner } from "@/shared/components/ui/spinner";
import {
  setFilterSorting,
  setInitialFilters,
  useGetVenuesFilter,
  useGetVenuesSorting,
} from "./model/venues-filter-store";
import {
  type DistanceOriginResult,
  useUserLocation,
} from "./model/use-user-location";
import { useCities } from "./model/use-cities";
import { VenueSorting } from "./venue-sorting";

export function VenuesList() {
  const {
    search,
    pageSize,
    hasLighting,
    onlyFree,
    onlyOpen,
    city,
    sportType,
    surface,
  } = useGetVenuesFilter();

  const { sortBy, sortDirection } = useGetVenuesSorting();

  const {
    coordinates: userCoordinates,
    status,
    errorCode,
    requestLocation,
  } = useUserLocation();

  const { data: cities, isPending: citiesPending } = useCities();

  const cityCenter = useMemo(() => {
    const cityData = cities?.result?.find((c) => c.name === city);
    return cityData
      ? { latitude: cityData.latitude, longitude: cityData.longitude }
      : null;
  }, [cities, city]);

  const origin = userCoordinates ?? cityCenter;
  const originSource: "geolocation" | "city" | null = userCoordinates
    ? "geolocation"
    : cityCenter
      ? "city"
      : null;

  const ensureDistanceOrigin =
    useCallback(async (): Promise<DistanceOriginResult> => {
      if (userCoordinates) {
        return {
          coordinates: userCoordinates,
          source: "geolocation",
          errorCode: null,
        };
      }

      if (status === "denied" || status === "unavailable") {
        return cityCenter
          ? { coordinates: cityCenter, source: "city", errorCode: null }
          : {
              coordinates: null,
              source: null,
              errorCode: errorCode ?? "no_city",
            };
      }

      const requested = await requestLocation();

      if (requested.coordinates) {
        return {
          coordinates: requested.coordinates,
          source: "geolocation",
          errorCode: null,
        };
      }

      return cityCenter
        ? { coordinates: cityCenter, source: "city", errorCode: null }
        : { coordinates: null, source: null, errorCode: requested.errorCode };
    }, [userCoordinates, status, errorCode, cityCenter, requestLocation]);

  const distanceReady = sortBy === "Distance" && origin !== null;
  const requestSortBy =
    sortBy === "Distance" && !distanceReady ? "CreatedAt" : sortBy;
  const requestSortDirection =
    sortBy === "Distance" && !distanceReady ? "Desc" : sortDirection;

  const { data, error, isFetchingNextPage, cursorRef } = useVenuesList({
    search: search === "" ? undefined : search,
    pageSize,
    hasLighting,
    onlyFree,
    onlyOpen,
    city,
    sportType,
    surface,
    sortBy: requestSortBy,
    sortDirection: requestSortDirection,
    latitude: distanceReady ? origin?.latitude : undefined,
    longitude: distanceReady ? origin?.longitude : undefined,
  });

  useEffect(() => {
    if (sortBy !== "Distance" || origin || citiesPending) return;
    setFilterSorting("CreatedAt", "Desc");
  }, [sortBy, origin, citiesPending]);

  const [selectedVenue, setSelectedVenue] = useState<Venue | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);

  const handleVenueClick = (venue: Venue) => {
    setSelectedVenue(venue);
    setDialogOpen(true);
  };

  if (error) {
    return <div>Ошибка: {error.message}</div>;
  }

  return (
    <div className="min-h-screen">
      <main className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-8">
        <div className="mb-8">
          <h1 className="text-3xl sm:text-4xl font-bold text-foreground mb-2 text-balance">
            Найди свою <span className="text-chart-3">площадку</span>
          </h1>
          <p className="text-muted-foreground text-lg">
            Большое количество спортивных площадок в твоём городе
          </p>
        </div>

        <div className="lg:flex gap-8">
          <FiltersSidebar />
          <div className="flex-1">
            <div className="flex flex-wrap items-center justify-between gap-3 mb-6">
              <p className="text-sm text-muted-foreground">
                Найдено:{" "}
                <span className="text-foreground font-medium">
                  {data?.venues.length}
                </span>{" "}
                площадок
              </p>
              <VenueSorting
                onEnsureOrigin={ensureDistanceOrigin}
                originSource={originSource}
              />
            </div>

            {data?.venues.length != undefined && data?.venues.length > 0 ? (
              <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-4 sm:gap-6">
                {data?.venues.map((venue) => (
                  <VenueCard
                    key={venue.id}
                    venue={venue}
                    onClick={handleVenueClick}
                  />
                ))}
              </div>
            ) : (
              <div className="flex flex-col items-center justify-center py-16 text-center">
                <div className="w-16 h-16 rounded-full bg-muted flex items-center justify-center mb-4">
                  <span className="text-2xl">🔍</span>
                </div>
                <h3 className="text-lg font-semibold text-foreground mb-2">
                  Площадки не найдены
                </h3>
                <p className="text-muted-foreground mb-4">
                  Попробуйте изменить параметры поиска
                </p>
                <button
                  onClick={setInitialFilters}
                  className="text-chart-3 hover:text-chart-3/80 font-medium transition-colors"
                >
                  Сбросить фильтры
                </button>
              </div>
            )}
          </div>
        </div>
      </main>

      <VenueDetailsDialog
        venue={selectedVenue}
        open={dialogOpen}
        onOpenChange={setDialogOpen}
      />

      <div ref={cursorRef} className="flex justify-center py-4">
        {isFetchingNextPage && <Spinner />}
      </div>
    </div>
  );
}
