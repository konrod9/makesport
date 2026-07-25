import { venuesQueryOptions } from "@/entities/venues/api";
import { useQuery } from "@tanstack/react-query";

export function useCities() {
  return useQuery(venuesQueryOptions.getCitiesQueryOptions());
}
