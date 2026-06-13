"use client";

import { venues } from "@/shared/lib/data";
import type { Venue } from "@/shared/lib/data";
import { FiltersSidebar } from "@/shared/components/filters-sidebar";
import { useMemo, useState } from "react";
import { VenueCard } from "@/shared/components/venue-card";
import { VenueDetailsDialog } from "@/shared/components/venue-details-dialog";

export default function Home() {
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedCity, setSelectedCity] = useState("all");
  const [selectedSport, setSelectedSport] = useState("all");
  const [selectedSurface, setSelectedSurface] = useState("all");
  const [onlyFree, setOnlyFree] = useState(false);
  const [onlyOpen, setOnlyOpen] = useState(false);
  const [hasLighting, setHasLighting] = useState(false);
  const [selectedVenue, setSelectedVenue] = useState<Venue | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);

  const handleVenueClick = (venue: Venue) => {
    setSelectedVenue(venue);
    setDialogOpen(true);
  };

  const filteredVenues = useMemo(() => {
    return venues.filter((venue) => {
      const matchesSearch =
        venue.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        venue.description.toLowerCase().includes(searchQuery.toLowerCase()) ||
        venue.address.toLowerCase().includes(searchQuery.toLowerCase());

      const matchesCity = selectedCity === "all" || venue.city === selectedCity;
      const matchesSport =
        selectedSport === "all" || venue.sportType === selectedSport;
      const matchesSurface =
        selectedSurface === "all" || venue.surface === selectedSurface;
      const matchesFree = !onlyFree || venue.isFree;
      const matchesOpen = !onlyOpen || venue.isOpen;
      const matchesLighting = !hasLighting || venue.hasLighting;

      return (
        matchesSearch &&
        matchesCity &&
        matchesSport &&
        matchesSurface &&
        matchesFree &&
        matchesOpen &&
        matchesLighting
      );
    });
  }, [
    searchQuery,
    selectedCity,
    selectedSport,
    selectedSurface,
    onlyFree,
    onlyOpen,
    hasLighting,
  ]);

  const resetFilters = () => {
    setSearchQuery("");
    setSelectedCity("all");
    setSelectedSport("all");
    setSelectedSurface("all");
    setOnlyFree(false);
    setOnlyOpen(false);
    setHasLighting(false);
  };

  return (
    <div className="min-h-screen">
      <main className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-8">
        {/* Hero Section */}
        <div className="mb-8">
          <h1 className="text-3xl sm:text-4xl font-bold text-foreground mb-2 text-balance">
            Найди свою <span className="text-chart-3">площадку</span>
          </h1>
          <p className="text-muted-foreground text-lg">
            Большое количество спортивных площадок в твоём городе
          </p>
        </div>

        <div className="lg:flex gap-8">
          <FiltersSidebar
            searchQuery={searchQuery}
            setSearchQuery={setSearchQuery}
            selectedCity={selectedCity}
            setSelectedCity={setSelectedCity}
            selectedSport={selectedSport}
            setSelectedSport={setSelectedSport}
            selectedSurface={selectedSurface}
            setSelectedSurface={setSelectedSurface}
            onlyFree={onlyFree}
            setOnlyFree={setOnlyFree}
            onlyOpen={onlyOpen}
            setOnlyOpen={setOnlyOpen}
            hasLighting={hasLighting}
            setHasLighting={setHasLighting}
            onReset={resetFilters}
          />

          <div className="flex-1">
            <div className="flex items-center justify-between mb-6">
              <p className="text-sm text-muted-foreground">
                Найдено:{" "}
                <span className="text-foreground font-medium">
                  {filteredVenues.length}
                </span>{" "}
                площадок
              </p>
            </div>

            {filteredVenues.length > 0 ? (
              <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-4 sm:gap-6">
                {filteredVenues.map((venue) => (
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
                  onClick={resetFilters}
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
    </div>
  );
}
