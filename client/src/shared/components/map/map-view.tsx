"use client";

import { Venue } from "@/entities/venues/types";
import { useCities } from "@/features/venues/model/use-cities";
import {
  setFilterCity,
  setFilterSearch,
  setFilterSportType,
  useGetVenuesFilter,
} from "@/features/venues/model/venues-filter-store";
import { sportTypes } from "@/shared/lib/data";
import { cn } from "@/shared/lib/utils";
import { YMaps, Map, Placemark } from "@pbe/react-yandex-maps";
import {
  ArrowUpRight,
  Crosshair,
  Locate,
  MapPin,
  Minus,
  Plus,
  Search,
  Star,
  X,
} from "lucide-react";
import Link from "next/link";
import { useMemo, useState } from "react";
import { VenuesMap } from "../map-page";
import { useVenuesListQuery } from "@/features/venues/model/use-venues-list-query";
import { VenueDetailsDialog } from "../venue-details-dialog";

const sportEmoji: Record<string, string> = {
  Футбол: "⚽",
  Баскетбол: "🏀",
  Теннис: "🎾",
  Воркаут: "🏋️",
  Волейбол: "🏐",
  Бег: "🏃",
  Велоспорт: "🚴",
  Скейтборд: "🛹",
};

function plural(n: number) {
  const mod10 = n % 10;
  const mod100 = n % 100;
  if (mod10 === 1 && mod100 !== 11) return "площадка";
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20))
    return "площадки";
  return "площадок";
}

export function MapView() {
  const [listOpen, setListOpen] = useState(false);
  const [activeId, setActiveId] = useState<string | null>(null);
  const [focus, setFocus] = useState<{ lat: number; lng: number } | null>(null);

  const [selectedVenue, setSelectedVenue] = useState<Venue | null>(null);
  const [detailsVenue, setDetailsVenue] = useState<Venue | null>(null);
  const [detailsOpen, setDetailsOpen] = useState(false);

  const handleShowDetails = (venue: Venue) => {
    setDetailsVenue(venue);
    setDetailsOpen(true);
    setSelectedVenue(null);
  };

  const { search, hasLighting, onlyFree, onlyOpen, city, sportType, surface } =
    useGetVenuesFilter();
  const { data: cities } = useCities();

  const { data, isPending, error, isError } = useVenuesListQuery({
    search: search === "" ? undefined : search,
    pageSize: 1000,
    hasLighting: hasLighting,
    onlyFree: onlyFree,
    onlyOpen: onlyOpen,
    city: city,
    sportType: sportType,
    surface: surface,
    sortBy: "CreatedAt",
    sortDirection: "Desc",
  });

  const center = useMemo(() => {
    const found = (cities?.result ?? []).find((c) => c.name === city);
    return found
      ? ([found.latitude, found.longitude] as [number, number])
      : undefined;
  }, [cities, city]);

  return (
    <div className="relative flex h-[calc(100vh-4.5rem)] min-h-[620px] w-full overflow-hidden">
      <div
        className={cn(
          "relative z-10 flex w-full shrink-0 flex-col border-r border-ink-200 bg-cream transition-all duration-300 sm:w-[380px] lg:w-[420px]",
          !listOpen && "hidden sm:flex",
        )}
      >
        <div className="border-b border-ink-200/70 p-5">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="font-display text-xl font-bold text-ink">
                Карта площадок
              </h1>
              <p className="mt-1 text-xs text-ink-400">
                Найдено {data?.venues.length}{" "}
                {plural(data?.venues.length ?? 1)}{" "}
              </p>
            </div>
            <button
              onClick={() => setListOpen(false)}
              className="grid size-9 place-items-center rounded-full border border-ink-200 text-ink-400 transition-colors hover:text-ink sm:hidden"
              aria-label="Скрыть список"
            >
              <X className="size-4" />
            </button>
          </div>

          <div className="relative mt-4">
            <Search className="pointer-events-none absolute left-3.5 top-1/2 size-4 -translate-y-1/2 text-ink-400" />
            <input
              value={search}
              onChange={(e) => setFilterSearch(e.target.value)}
              placeholder="Поиск по названию или адресу"
              className="h-11 w-full rounded-2xl border border-ink-200 bg-white pl-10 pr-4 text-sm outline-none transition-all placeholder:text-ink-400 focus:border-ink focus:ring-4 focus:ring-lime/30"
            />
          </div>

          <div className="mt-3 flex gap-2">
            <select
              value={city}
              onChange={(e) => {
                setFilterCity(e.target.value);
                setActiveId(null);
              }}
              className="h-10 flex-1 rounded-xl border border-ink-200 bg-white px-3 text-xs font-semibold outline-none focus:border-ink"
            >
              <option value="">Все</option>
              {cities?.result?.map((c) => (
                <option key={c.name}>{c.name}</option>
              ))}
            </select>
            <select
              value={sportType}
              onChange={(e) => setFilterSportType(e.target.value)}
              className="h-10 flex-1 rounded-xl border border-ink-200 bg-white px-3 text-xs font-semibold outline-none focus:border-ink"
            >
              <option value="">Все</option>
              {sportTypes.map((s) => (
                <option key={s}>{s}</option>
              ))}
            </select>
          </div>

          <div className="no-scrollbar mt-3 flex gap-1.5 overflow-x-auto pb-1">
            {["Все", "Футбол", "Баскетбол", "Воркаут", "Теннис", "Бег"].map(
              (s) => (
                <button
                  key={s}
                  onClick={() => setFilterSportType(s === "Все" ? "" : s)}
                  className={cn(
                    "whitespace-nowrap rounded-full px-3 py-1.5 text-[11px] font-semibold transition-colors",
                    (sportType || "Все") === s
                      ? "bg-ink text-cream"
                      : "bg-ink-100 text-ink-500 hover:bg-ink-200",
                  )}
                >
                  {s}
                </button>
              ),
            )}
          </div>
        </div>

        <div className="no-scrollbar flex-1 space-y-2 overflow-y-auto p-3">
          {data?.venues.length === 0 && (
            <div className="p-8 text-center">
              <p className="text-sm font-semibold text-ink">
                Ничего не найдено
              </p>
              <p className="mt-1 text-xs text-ink-400">
                Измените фильтры или добавьте площадку.
              </p>
            </div>
          )}
          {data?.venues.map((v) => (
            <button
              key={v.id}
              onClick={() => {
                setActiveId(v.id);
                setFocus({
                  lat: v.coordinates.latitude,
                  lng: v.coordinates.longitude,
                });
              }}
              className={cn(
                "flex w-full gap-3 rounded-2xl border p-3 text-left transition-all",
                activeId === v.id
                  ? "border-ink bg-white shadow-card"
                  : "border-transparent bg-white/60 hover:border-ink-200 hover:bg-white",
              )}
            >
              <div className="relative size-20 shrink-0 overflow-hidden rounded-xl bg-ink-100">
                {/* eslint-disable-next-line @next/next/no-img-element */}
                <img
                  src={v.images[0].url}
                  alt={v.title}
                  className="size-full object-cover"
                />
              </div>
              <div className="min-w-0 flex-1">
                <div className="flex items-start justify-between gap-2">
                  <p className="line-clamp-1 text-[13px] font-semibold text-ink">
                    {v.title}
                  </p>
                  <span className="flex shrink-0 items-center gap-0.5 text-[11px] font-bold text-ink">
                    <Star className="size-3 fill-lime-500 text-lime-500" />
                    {v.rating}
                  </span>
                </div>
                <p className="mt-1 line-clamp-1 text-[11px] text-ink-400">
                  {v.address.city}, {v.address.street}
                </p>
                <div className="mt-2 flex flex-wrap items-center gap-1">
                  {/* {v.sports.slice(0, 2).map((s) => (
                    <span
                      key={s}
                      className="rounded-full bg-ink-50 px-2 py-0.5 text-[10px] font-semibold text-ink-500"
                    >
                      {s}
                    </span>
                  ))} */}
                  <span className="rounded-full bg-ink-50 px-2 py-0.5 text-[10px] font-semibold text-ink-500">
                    {v.sportType}
                  </span>
                  <span className="ml-auto text-[10px] font-semibold text-ink-400">
                    {v.distanceMeters}
                  </span>
                </div>
              </div>
            </button>
          ))}
        </div>

        <div className="border-t border-ink-200/70 p-4">
          <Link
            href="/add"
            className="flex items-center justify-center gap-2 rounded-2xl bg-lime py-3 text-sm font-bold text-ink transition-transform hover:-translate-y-0.5"
          >
            <MapPin className="size-4" /> Добавить площадку
          </Link>
        </div>
      </div>
      <VenuesMap
        venues={data?.venues ?? []}
        center={center}
        selectedVenue={selectedVenue}
        onSelectVenue={setSelectedVenue}
        onShowDetails={handleShowDetails}
      />
      <VenueDetailsDialog
        venue={detailsVenue}
        open={detailsOpen}
        onOpenChange={setDetailsOpen}
      />
    </div>
  );
}
