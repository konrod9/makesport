import { apiClient } from "@/shared/api/axios-instance";
import { Envelope } from "@/shared/api/envelope";
import { PaginationVenuesResponse } from "@/shared/api/types";
import { AddressDto, CoordinatesDto, Venue, WorkingHoursDto } from "./types";
import { infiniteQueryOptions } from "@tanstack/react-query";

export type GetVenuesRequest = {
  search?: string;
  page: number;
  pageSize: number;
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

  getVenuesInfiniteOptions: ({
    pageSize,
    searchQuery,
  }: {
    pageSize: number;
    searchQuery?: string;
  }) => {
    return infiniteQueryOptions({
      queryKey: [venuesQueryOptions.baseKey, searchQuery],
      queryFn: ({ pageParam }) => {
        return venuesApi.getVenues({
          search: searchQuery,
          page: pageParam,
          pageSize,
        });
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
        pageSize: data.pages[0]?.pageSize ?? pageSize,
        totalPages: data.pages[0]?.totalPages ?? 0,
      }),
    });
  },
};
