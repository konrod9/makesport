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
  workingHours: WorkingHoursDto;
  video?: MediaDto;
  address: AddressDto;
  coordinates: CoordinatesDto;
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
  longitue: number;
};

export type MediaStatus =
  | "uploading"
  | "uploaded"
  | "ready"
  | "failed"
  | "deleted";
