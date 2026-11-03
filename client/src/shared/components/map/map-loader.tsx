"use client";

import { MediaDto, Venue } from "@/entities/venues/types";
import dynamic from "next/dynamic";

const MapView = dynamic(
  () => import("@/shared/components/map/map-view").then((mod) => mod.MapView),
  {
    ssr: false,
    loading: () => (
      <div className="h-full w-full flex items-center justify-center bg-card rounded-lg">
        <div className="text-muted-foreground">Загрузка карты...</div>
      </div>
    ),
  },
);

// Хелпер для медиа — чтобы не дублировать
const makeMedia = (id: string, url: string): MediaDto => ({
  id,
  url,
  status: "ready",
});

export const mockVenues: Venue[] = [
  {
    id: "venue-1",
    title: "Спортзал «Олимп»",
    description:
      "Современный зал с новым покрытием и тренажёрами. Идеально для баскетбола и волейбола.",
    sportType: "Баскетбол",
    surface: "Паркет",
    rating: 4.8,
    reviewCount: 214,
    images: [
      makeMedia("m-1-1", "https://picsum.photos/seed/venue1a/800/600"),
      makeMedia("m-1-2", "https://picsum.photos/seed/venue1b/800/600"),
      makeMedia("m-1-3", "https://picsum.photos/seed/venue1c/800/600"),
    ],
    isOpen: true,
    hasLighting: true,
    isFree: false,
    workingHours: "08:00–22:00",
    video: makeMedia("m-1-v", "https://www.w3schools.com/html/mov_bbb.mp4"),
    address: {
      city: "Москва",
      street: "Ленинский проспект",
      building: "42",
      fullName: "Москва, Ленинский проспект, 42",
    },
    coordinates: { latitude: 55.7083, longitude: 37.5823 },
    createdAt: "2024-03-12T10:20:00.000Z",
    distanceMeters: 420,
  },
  {
    id: "venue-2",
    title: "Открытая площадка «Пионер»",
    description:
      "Уличная площадка с резиновым покрытием. Бесплатная, работает круглосуточно.",
    sportType: "Футбол",
    surface: "Резина",
    rating: 4.3,
    reviewCount: 87,
    images: [
      makeMedia("m-2-1", "https://picsum.photos/seed/venue2a/800/600"),
      makeMedia("m-2-2", "https://picsum.photos/seed/venue2b/800/600"),
    ],
    isOpen: true,
    hasLighting: false,
    isFree: true,
    workingHours: "Круглосуточно",
    address: {
      city: "Москва",
      street: "ул. Пионерская",
      building: "15",
      fullName: "Москва, ул. Пионерская, 15",
    },
    coordinates: { latitude: 55.7365, longitude: 37.5219 },
    createdAt: "2024-01-04T09:00:00.000Z",
    distanceMeters: 1250,
  },
  {
    id: "venue-3",
    title: "Теннисный клуб «Ace»",
    description:
      "Крытые корты с профессиональным покрытием. Есть тренер и прокат инвентаря.",
    sportType: "Теннис",
    surface: "Хард",
    rating: 4.9,
    reviewCount: 512,
    images: [
      makeMedia("m-3-1", "https://picsum.photos/seed/venue3a/800/600"),
      makeMedia("m-3-2", "https://picsum.photos/seed/venue3b/800/600"),
      makeMedia("m-3-3", "https://picsum.photos/seed/venue3c/800/600"),
      makeMedia("m-3-4", "https://picsum.photos/seed/venue3d/800/600"),
    ],
    isOpen: true,
    hasLighting: true,
    isFree: false,
    workingHours: "07:00–23:00",
    video: makeMedia("m-3-v", "https://www.w3schools.com/html/mov_bbb.mp4"),
    address: {
      city: "Москва",
      street: "Кутузовский проспект",
      building: "88",
      fullName: "Москва, Кутузовский проспект, 88",
    },
    coordinates: { latitude: 55.7401, longitude: 37.5072 },
    createdAt: "2023-11-21T14:45:00.000Z",
    distanceMeters: 2100,
  },
  {
    id: "venue-4",
    title: "Скейт-парк «Volt»",
    description:
      "Современный скейт-парк с рампой, грайндбоксами и освещением для вечерних сессий.",
    sportType: "Скейтбординг",
    surface: "Бетон",
    rating: 4.5,
    reviewCount: 143,
    images: [
      makeMedia("m-4-1", "https://picsum.photos/seed/venue4a/800/600"),
      makeMedia("m-4-2", "https://picsum.photos/seed/venue4b/800/600"),
    ],
    isOpen: true,
    hasLighting: true,
    isFree: true,
    workingHours: "06:00–00:00",
    address: {
      city: "Москва",
      street: "Парк Горького",
      fullName: "Москва, Парк Горького",
    },
    coordinates: { latitude: 55.7304, longitude: 37.6012 },
    createdAt: "2024-05-08T16:10:00.000Z",
    distanceMeters: 890,
  },
  {
    id: "venue-5",
    title: "Спортивный комплекс «Атлант»",
    description:
      "Многофункциональный комплекс: волейбол, баскетбол, мини-футбол. Раздевалки и душевые.",
    sportType: "Волейбол",
    surface: "Паркет",
    rating: 4.1,
    reviewCount: 61,
    images: [
      makeMedia("m-5-1", "https://picsum.photos/seed/venue5a/800/600"),
      makeMedia("m-5-2", "https://picsum.photos/seed/venue5b/800/600"),
      makeMedia("m-5-3", "https://picsum.photos/seed/venue5c/800/600"),
    ],
    isOpen: false,
    hasLighting: true,
    isFree: false,
    workingHours: "09:00–21:00",
    address: {
      city: "Москва",
      street: "ул. Профсоюзная",
      building: "126",
      fullName: "Москва, ул. Профсоюзная, 126",
    },
    coordinates: { latitude: 55.6698, longitude: 37.5601 },
    createdAt: "2024-02-15T11:30:00.000Z",
    distanceMeters: null, // не определено (геолокация недоступна)
  },
  {
    id: "venue-6",
    title: "Стол для пинг-понга «Spin»",
    description: "Уличные столы для настольного тенниса под навесом.",
    sportType: "Настольный теннис",
    surface: "Синтетика",
    rating: 4.0,
    reviewCount: 23,
    images: [makeMedia("m-6-1", "https://picsum.photos/seed/venue6a/800/600")],
    isOpen: true,
    hasLighting: false,
    isFree: true,
    workingHours: "09:00–20:00",
    address: {
      city: "Москва",
      street: "Сокольнический вал",
      building: "1",
      fullName: "Москва, Сокольнический вал, 1",
    },
    coordinates: { latitude: 55.7902, longitude: 37.6751 },
    createdAt: "2024-06-01T08:00:00.000Z",
    distanceMeters: 3450,
  },
];

export function MapLoader() {
  return <MapView venues={mockVenues} />;
}
