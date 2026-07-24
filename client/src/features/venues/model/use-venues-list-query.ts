import { useDebounce } from "use-debounce";
import { UseVenuesListParams } from "./use-venues-list";
import { useQuery } from "@tanstack/react-query";
import { venuesQueryOptions } from "@/entities/venues/api";

export function useVenuesListQuery({
  search,
  pageSize,
  hasLighting,
  onlyFree,
  onlyOpen,
  city,
  sportType,
  surface,
}: UseVenuesListParams) {
  const [debouncedSearch] = useDebounce(search, 300);

  return useQuery({
    ...venuesQueryOptions.getVenuesQueryOptions({
      search: debouncedSearch,
      pageSize,
      hasLighting: hasLighting || undefined,
      onlyFree: onlyFree || undefined,
      onlyOpen: onlyOpen || undefined,
      city: city === "" ? undefined : city,
      sportType: sportType === "" ? undefined : sportType,
      surface: surface === "" ? undefined : surface,
    }),
  });
}
