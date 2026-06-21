import { venuesQueryOptions } from "@/entities/venues/api";
import { EnvelopeError } from "@/shared/api/errors";
import { useInfiniteQuery } from "@tanstack/react-query";
import { RefCallback, useCallback } from "react";

export function useVenuesList(pageSize: number) {
  const {
    data,
    isPending,
    error,
    isError,
    fetchNextPage,
    isFetchingNextPage,
    hasNextPage,
  } = useInfiniteQuery({
    ...venuesQueryOptions.getVenuesInfiniteOptions({ pageSize: pageSize }),
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
