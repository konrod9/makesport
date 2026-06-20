import { apiClient } from "@/shared/api/axios-instance";
import { Envelope } from "@/shared/api/envelope";
import { PaginationVenuesResponse } from "@/shared/api/types";
import { AddressDto, CoordinatesDto, Venue } from "./types";

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
  workingHours?: string;
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
};
