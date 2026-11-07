import { venuesQueryOptions } from "@/entities/venues/api";
import { useQuery } from "@tanstack/react-query";

export function useMyVenues() {
  return useQuery(venuesQueryOptions.getMyVenuesQueryOptions());
}
