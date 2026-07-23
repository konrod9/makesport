import { apiClient } from "@/shared/api/axios-instance";
import { Envelope } from "@/shared/api/envelope";
import { PaginationVenuesResponse } from "@/shared/api/types";
import { AddressDto, CoordinatesDto, Venue, WorkingHoursDto } from "./types";
import { infiniteQueryOptions, queryOptions } from "@tanstack/react-query";
import { VenuesFilterState } from "@/features/venues/model/venues-filter-store";

export type GetVenuesRequest = {
  search?: string;
  page: number;
  pageSize: number;
  hasLighting?: boolean;
  onlyFree?: boolean;
  onlyOpen?: boolean;
  city?: string;
  sportType?: string;
  surface?: string;
};

export type CreateVenueRequest = {
  title: string;
  description?: string;
  address: AddressDto;
  coordinates: CoordinatesDto;
  sportType: string;
  surface: string;
  isOpen: boolean;
  hasLighting: boolean;
  isFree: boolean;
  workingHours: WorkingHoursDto;
  images?: string[];
};

export const venuesApi = {
  getVenues: async (request: GetVenuesRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationVenuesResponse<Venue>>
    >("/venues", {
      params: request,
    });

    return response.data.result;
  },

  createVenue: async (request: CreateVenueRequest) => {
    const response = await apiClient.post<Envelope<string>>("/venues", request);

    return response.data;
  },
};

export const venuesQueryOptions = {
  baseKey: "venues",

  getVenuesInfiniteOptions: (filter: VenuesFilterState) => {
    return infiniteQueryOptions({
      queryKey: [venuesQueryOptions.baseKey, filter],
      queryFn: ({ pageParam }) => {
        return venuesApi.getVenues({ ...filter, page: pageParam });
      },
      initialPageParam: 1,
      getNextPageParam: (response) => {
        if (!response || response.page >= response.totalPages) return undefined;
        return response.page + 1;
      },
      select: (data): PaginationVenuesResponse<Venue> => ({
        venues: data.pages.flatMap((page) => page?.venues ?? []),
        totalCount: data.pages[0]?.totalCount ?? 0,
        page: data.pages[0]?.page ?? 1,
        pageSize: data.pages[0]?.pageSize ?? filter.pageSize,
        totalPages: data.pages[0]?.totalPages ?? 0,
      }),
    });
  },

  getVenuesQueryOptions: (filter: VenuesFilterState) => {
    return queryOptions({
      queryKey: [venuesQueryOptions.baseKey, filter],
      queryFn: () => venuesApi.getVenues({ ...filter, page: 1 }),
    });
  },
};
