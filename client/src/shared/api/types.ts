export type PaginationVenuesResponse<T> = {
  venues: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
};

export type VenuesResponse<T> = {
  venues: T[];
  totalCount: number;
};
