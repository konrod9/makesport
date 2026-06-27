import { venuesQueryOptions } from "@/entities/venues/api";
import { EnvelopeError } from "@/shared/api/errors";
import { useInfiniteQuery } from "@tanstack/react-query";
import { RefCallback, useCallback } from "react";
import { useDebounce } from "use-debounce";
import { VenuesFilterState } from "./venues-filter-store";

export const PAGE_SIZE = 2;

type UseVenuesListParams = VenuesFilterState & {
  pageSize: number;
};

export function useVenuesList({
  search,
  pageSize,
  hasLighting,
}: UseVenuesListParams) {
  const [debouncedSearch] = useDebounce(search, 300);

  const {
    data,
    isPending,
    error,
    isError,
    fetchNextPage,
    isFetchingNextPage,
    hasNextPage,
  } = useInfiniteQuery({
    ...venuesQueryOptions.getVenuesInfiniteOptions({
      pageSize: PAGE_SIZE,
      search: debouncedSearch,
      hasLighting: hasLighting ? hasLighting : undefined,
    }),
  });

  const cursorRef: RefCallback<HTMLDivElement> = useCallback(
    (el) => {
      const observer = new IntersectionObserver(
        (entries) => {
          if (entries[0].isIntersecting && hasNextPage && !isFetchingNextPage) {
            fetchNextPage();
          }
        },
        { threshold: 0.5 },
      );

      if (el) {
        observer.observe(el);

        return () => observer.disconnect();
      }
    },
    [fetchNextPage, hasNextPage, isFetchingNextPage],
  );

  return {
    data: data,
    totalPages: data?.totalPages,
    totalCount: data?.totalCount,
    isPending,
    error: error instanceof EnvelopeError ? error : undefined,
    isError: isError,
    isFetchingNextPage,
    cursorRef,
  };
}
