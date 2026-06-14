import { apiClient } from "@/shared/api/axios-instance";
import { Envelope } from "@/shared/api/envelope";
import { PaginationVenuesResponse } from "@/shared/api/types";
import { Venue } from "./types";

export type GetLessonsRequest = {
  search?: string;
  page: number;
  pageSize: number;
};

export const venuesApi = {
  getVenues: async (request: GetLessonsRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationVenuesResponse<Venue>>
    >("/venues", {
      params: request,
    });

    return response.data.result;
  },
};
