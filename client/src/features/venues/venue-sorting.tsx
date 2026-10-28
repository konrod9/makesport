"use client";

import { useState } from "react";
import {
  DistanceOriginResult,
  GeolocationErrorCode,
} from "./model/use-user-location";
import {
  setFilterSorting,
  useGetVenuesSorting,
} from "./model/venues-filter-store";
import { SortDirection, VenueSortBy } from "@/entities/venues/types";
import { Loader2 } from "lucide-react";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select";

type SortOption = {
  value: string;
  label: string;
  sortBy: VenueSortBy;
  sortDirection: SortDirection;
};

const SORT_OPTIONS: SortOption[] = [
  {
    value: "createdAt:desc",
    label: "Сначала новые",
    sortBy: "CreatedAt",
    sortDirection: "Desc",
  },
  {
    value: "createdAt:asc",
    label: "Сначала старые",
    sortBy: "CreatedAt",
    sortDirection: "Asc",
  },
  {
    value: "title:asc",
    label: "По названию А-Я",
    sortBy: "Title",
    sortDirection: "Asc",
  },
  {
    value: "title:desc",
    label: "По названию Я-А",
    sortBy: "Title",
    sortDirection: "Desc",
  },
  {
    value: "distance:asc",
    label: "Сначала ближние",
    sortBy: "Distance",
    sortDirection: "Asc",
  },
  {
    value: "distance:desc",
    label: "Сначала дальние",
    sortBy: "Distance",
    sortDirection: "Desc",
  },
];

const ERROR_MESSAGES: Record<GeolocationErrorCode | "no_city", string> = {
  unsupported: "Браузер не поддерживает геолокацию",
  insecure_context: "Геолокация доступна только по HTTPS",
  permission_denied: "Вы запретили доступ к геолокации",
  position_unavailable: "Не удалось определить местоположение",
  timeout: "Не удалось получить геолокацию вовремя",
  unknown: "Не удалось определить местоположение",
  no_city: "Выберите город или разрешите геолокацию",
};

type VenueSortngProps = {
  onEnsureOrigin: () => Promise<DistanceOriginResult>;
  originSource: "geolocation" | "city" | null;
};

export function VenueSorting({
  onEnsureOrigin,
  originSource,
}: VenueSortngProps) {
  const { sortBy, sortDirection } = useGetVenuesSorting();
  const [isResolving, setIsResolving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const selectedValue =
    SORT_OPTIONS.find(
      (option) =>
        option.sortBy === sortBy && option.sortDirection === sortDirection,
    )?.value ?? SORT_OPTIONS[0].value;

  const handleChange = async (value: string) => {
    const option = SORT_OPTIONS.find((item) => item.value === value);
    if (!option || isResolving) return;

    setError(null);

    if (option.sortBy !== "Distance") {
      setFilterSorting(option.sortBy, option.sortDirection);
      return;
    }

    setIsResolving(true);

    try {
      const result = await onEnsureOrigin();

      if (!result.coordinates) {
        setError(ERROR_MESSAGES[result.errorCode]);
        return;
      }

      setFilterSorting("Distance", option.sortDirection);
    } catch {
      setError(ERROR_MESSAGES.unknown);
    } finally {
      setIsResolving(false);
    }
  };

  return (
    <div className="flex flex-col items-end gap-1">
      <div className="flex items-center gap-2">
        {isResolving && (
          <Loader2 className="h-4 w-4 animate-spin text-muted-foreground" />
        )}
        <Select
          value={selectedValue}
          onValueChange={handleChange}
          disabled={isResolving}
        >
          <SelectTrigger className="w-[200px] bg-input border-border">
            <SelectValue placeholder="Сортировка" />
          </SelectTrigger>
          <SelectContent>
            {SORT_OPTIONS.map((option) => (
              <SelectItem key={option.value} value={option.value}>
                {option.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      {originSource && sortBy === "Distance" && !error && (
        <span className="text-xs text-muted-foreground">
          {originSource === "geolocation"
            ? "Рядом с вами"
            : "Рядом с центром города"}
        </span>
      )}

      {error && (
        <span className="text-xs text-destructive" role="alert">
          {error}
        </span>
      )}
    </div>
  );
}
