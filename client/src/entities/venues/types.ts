export type Venue = {
  id: string;
  title: string;
  description: string;
  sportType: string;
  surface: string;
  rating: number;
  reviewCount: number;
  images: MediaDto[];
  isOpen: boolean;
  hasLighting: boolean;
  isFree: boolean;
  workingHours: string;
  video?: MediaDto;
  address: AddressDto;
  coordinates: CoordinatesDto;
  createdAt: string;
  distanceMeters: number | null;
};

export type WorkingHoursDto = {
  workingStart: string;
  workingEnd: string;
};

export type MediaDto = {
  id: string;
  url: string;
  status: MediaStatus;
};

export type AddressDto = {
  city: string;
  street: string;
  building?: string;
  fullName: string;
};

export type CoordinatesDto = {
  latitude: number;
  longitude: number;
};

export type CityDto = {
  name: string;
} & CoordinatesDto;

export type MediaStatus =
  | "uploading"
  | "uploaded"
  | "ready"
  | "failed"
  | "deleted";

export type VenueSortBy = "CreatedAt" | "Title" | "Distance";

export type SortDirection = "Asc" | "Desc";
