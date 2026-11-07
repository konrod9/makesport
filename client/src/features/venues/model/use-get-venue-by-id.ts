import { venuesQueryOptions } from "@/entities/venues/api";
import { useQuery } from "@tanstack/react-query";

export function useGetVenueById(id: string) {
  return useQuery(venuesQueryOptions.getByIdQueryOptions(id));
}
