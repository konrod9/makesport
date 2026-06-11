type Venue = {
  id: string;
  title: string;
  description: string;
  video?: MediaDto;
  address: AddressDto;
  coordinates: CoordinatesDto;
};

type MediaDto = {
  id: string;
  url: string;
  status: MediaStatus;
};

type AddressDto = {
  city: string;
  street: string;
  building?: string;
};

type CoordinatesDto = {
  latitude: number;
  longitue: number;
};

export type MediaStatus =
  | "uploading"
  | "uploaded"
  | "ready"
  | "failed"
  | "deleted";
