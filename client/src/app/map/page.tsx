"use client";

import { useState, useMemo } from "react";
import dynamic from "next/dynamic";
import { SlidersHorizontal, Search } from "lucide-react";
import { FiltersContent } from "@/shared/components/filters-sidebar";
import { VenueDetailsDialog } from "@/shared/components/venue-details-dialog";
import { Input } from "@/shared/components/ui/input";
import { Button } from "@/shared/components/ui/button";
import { Badge } from "@/shared/components/ui/badge";
import { venues } from "@/shared/lib/data";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/shared/components/ui/sheet";
import { VenuesMap } from "@/shared/components/map-page";

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
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedCity, setSelectedCity] = useState("all");
  const [selectedSport, setSelectedSport] = useState("all");
  const [selectedSurface, setSelectedSurface] = useState("all");
  const [onlyFree, setOnlyFree] = useState(false);
  const [onlyOpen, setOnlyOpen] = useState(false);
  const [hasLighting, setHasLighting] = useState(false);
  const [selectedVenue, setSelectedVenue] = useState<Venue | null>(null);
  const [detailsVenue, setDetailsVenue] = useState<Venue | null>(null);
  const [detailsOpen, setDetailsOpen] = useState(false);

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

  const handleReset = () => {
    setSearchQuery("");
    setSelectedCity("all");
    setSelectedSport("all");
    setSelectedSurface("all");
    setOnlyFree(false);
    setOnlyOpen(false);
    setHasLighting(false);
  };

  const handleShowDetails = (venue: Venue) => {
    setDetailsVenue(venue);
    setDetailsOpen(true);
    setSelectedVenue(null);
  };

  const activeFilterCount =
    (selectedCity !== "all" ? 1 : 0) +
    (selectedSport !== "all" ? 1 : 0) +
    (selectedSurface !== "all" ? 1 : 0) +
    (onlyFree ? 1 : 0) +
    (onlyOpen ? 1 : 0) +
    (hasLighting ? 1 : 0);

  const filterProps = {
    selectedCity,
    setSelectedCity,
    selectedSport,
    setSelectedSport,
    selectedSurface,
    setSelectedSurface,
    onlyFree,
    setOnlyFree,
    onlyOpen,
    setOnlyOpen,
    hasLighting,
    setHasLighting,
    onReset: handleReset,
  };

  const searchInput = (
    <div className="relative">
      <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
      <Input
        placeholder="Поиск площадок..."
        value={searchQuery}
        onChange={(e) => setSearchQuery(e.target.value)}
        className="pl-9 bg-input border-border"
      />
    </div>
  );

  return (
    <div className="min-h-screen bg-background flex flex-col">
      <main className="flex-1 flex flex-col lg:flex-row">
        {/* Desktop Filters Sidebar */}
        <aside className="hidden lg:block lg:w-80 shrink-0 p-6 lg:border-r border-border overflow-y-auto">
          <div className="space-y-4">
            {searchInput}
            <div className="p-4 rounded-lg border border-border bg-card">
              <FiltersContent />
            </div>
            <div className="p-3 rounded-lg bg-card border border-border">
              <p className="text-sm text-muted-foreground">
                Найдено площадок:{" "}
                <span className="text-foreground font-medium">
                  {filteredVenues.length}
                </span>
              </p>
            </div>
          </div>
        </aside>

        {/* Mobile/Tablet Filter Bar */}
        <div className="lg:hidden flex items-center gap-3 p-4 border-b border-border">
          <div className="flex-1">{searchInput}</div>
          <Sheet>
            <SheetTrigger asChild>
              <Button variant="outline" className="relative shrink-0">
                <SlidersHorizontal className="h-4 w-4" />
                Фильтры
                {activeFilterCount > 0 && (
                  <Badge className="ml-1 h-5 min-w-5 px-1 bg-primary text-primary-foreground">
                    {activeFilterCount}
                  </Badge>
                )}
              </Button>
            </SheetTrigger>
            <SheetContent side="left" className="w-80 overflow-y-auto">
              <SheetHeader>
                <SheetTitle>Фильтры</SheetTitle>
              </SheetHeader>
              <div className="mt-6 px-4 pb-6">
                <FiltersContent {...filterProps} hideTitle />
              </div>
            </SheetContent>
          </Sheet>
        </div>

        {/* Map Area */}
        <div className="flex-1 p-4 lg:p-6">
          <div className="lg:hidden mb-3 text-sm text-muted-foreground">
            Найдено площадок:{" "}
            <span className="text-foreground font-medium">
              {filteredVenues.length}
            </span>
          </div>
          <div className="h-[calc(100vh-13rem)] lg:h-[calc(100vh-7rem)] rounded-lg overflow-hidden border border-border">
            <VenueMap
              venues={filteredVenues}
              selectedVenue={selectedVenue}
              onSelectVenue={setSelectedVenue}
              onShowDetails={handleShowDetails}
            />
            {/* <VenuesMap /> */}
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
