"use client";

import { useState } from "react";
import dynamic from "next/dynamic";
import { FiltersSidebar } from "@/shared/components/filters-sidebar";
import { VenueDetailsDialog } from "@/shared/components/venue-details-dialog";
import { useGetVenuesFilter } from "@/features/venues/model/venues-filter-store";
import { Venue } from "@/entities/venues/types";
import { useVenuesListQuery } from "@/features/venues/model/use-venues-list-query";

const VenueMap = dynamic(
  () => import("@/shared/components/map-page").then((mod) => mod.VenuesMap),
  {
    ssr: false,
    loading: () => (
      <div className="h-full w-full flex items-center justify-center bg-card rounded-lg">
        <div className="text-muted-foreground">Загрузка карты...</div>
      </div>
    ),
  },
);

export default function MapPage() {
  const { search, hasLighting, onlyFree, onlyOpen, city, sportType, surface } =
    useGetVenuesFilter();

  const { data, isPending, error, isError } = useVenuesListQuery({
    search: search === "" ? undefined : search,
    pageSize: 1000,
    hasLighting: hasLighting,
    onlyFree: onlyFree,
    onlyOpen: onlyOpen,
    city: city,
    sportType: sportType,
    surface: surface,
  });

  const [selectedVenue, setSelectedVenue] = useState<Venue | null>(null);
  const [detailsVenue, setDetailsVenue] = useState<Venue | null>(null);
  const [detailsOpen, setDetailsOpen] = useState(false);

  const handleShowDetails = (venue: Venue) => {
    setDetailsVenue(venue);
    setDetailsOpen(true);
    setSelectedVenue(null);
  };

  if (isPending) return <div>Загрузка карты...</div>;
  if (isError) return <div>Ошибка: {error?.message}</div>;

  return (
    <div className="min-h-screen bg-background flex flex-col">
      <main className="flex-1 flex flex-col lg:flex-row">
        <FiltersSidebar />

        {/* Map Area */}
        <div className="flex-1 p-4 lg:p-6">
          <div className="lg:hidden mb-3 text-sm text-muted-foreground">
            Найдено площадок:{" "}
            <span className="text-foreground font-medium">
              {data?.venues.length}
            </span>
          </div>
          <div className="h-[calc(100vh-13rem)] lg:h-[calc(100vh-7rem)] rounded-lg overflow-hidden border border-border">
            <VenueMap
              venues={data?.venues ?? []}
              selectedVenue={selectedVenue}
              onSelectVenue={setSelectedVenue}
              onShowDetails={handleShowDetails}
            />
          </div>
        </div>
      </main>

      <VenueDetailsDialog
        venue={detailsVenue}
        open={detailsOpen}
        onOpenChange={setDetailsOpen}
      />
    </div>
  );
}
