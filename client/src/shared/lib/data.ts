export interface Venue {
  id: string;
  name: string;
  description: string;
  city: string;
  address: string;
  sportType: string;
  surface: string;
  rating: number;
  reviewCount: number;
  image: string;
  isOpen: boolean;
  hasLighting: boolean;
  isFree: boolean;
  workingHours: string;
  coordinates: {
    lat: number;
    lng: number;
  };
}

export const venues: Venue[] = [
  {
    id: "1",
    name: "Баскетбольная площадка Парк Горького",
    description:
      "Современная площадка с профессиональным покрытием и освещением для вечерних игр.",
    city: "Москва",
    address: "ул. Крымский Вал, 9",
    sportType: "Баскетбол",
    surface: "Резиновое",
    rating: 4.8,
    reviewCount: 124,
    image: "/venues/basketball-1.jpg",
    isOpen: true,
    hasLighting: true,
    isFree: true,
    workingHours: "06:00 - 23:00",
    coordinates: { lat: 55.7298, lng: 37.6001 },
  },
  {
    id: "2",
    name: "Футбольное поле Лужники",
    description:
      "Большое футбольное поле с искусственным газоном. Раздевалки и душевые.",
    city: "Москва",
    address: "ул. Лужники, 24",
    sportType: "Футбол",
    surface: "Искусственный газон",
    rating: 4.6,
    reviewCount: 89,
    image: "/venues/football-1.jpg",
    isOpen: true,
    hasLighting: true,
    isFree: false,
    workingHours: "08:00 - 22:00",
    coordinates: { lat: 55.7155, lng: 37.5536 },
  },
  {
    id: "3",
    name: "Теннисные корты Динамо",
    description: "4 профессиональных теннисных корта с грунтовым покрытием.",
    city: "Москва",
    address: "Ленинградский проспект, 36",
    sportType: "Теннис",
    surface: "Грунт",
    rating: 4.9,
    reviewCount: 56,
    image: "/venues/tennis-1.jpg",
    isOpen: false,
    hasLighting: true,
    isFree: false,
    workingHours: "07:00 - 21:00",
    coordinates: { lat: 55.7911, lng: 37.5614 },
  },
  {
    id: "4",
    name: "Воркаут-площадка ВДНХ",
    description:
      "Открытая площадка для уличных тренировок с турниками и брусьями.",
    city: "Москва",
    address: "пр-т Мира, 119",
    sportType: "Воркаут",
    surface: "Резиновое",
    rating: 4.5,
    reviewCount: 203,
    image: "/venues/workout-1.jpg",
    isOpen: true,
    hasLighting: false,
    isFree: true,
    workingHours: "Круглосуточно",
    coordinates: { lat: 55.8267, lng: 37.6376 },
  },
  {
    id: "5",
    name: "Волейбольная площадка Сокольники",
    description:
      "Песчаная волейбольная площадка в парке. Сетка и разметка в комплекте.",
    city: "Москва",
    address: "Сокольнический вал, 1",
    sportType: "Волейбол",
    surface: "Песок",
    rating: 4.3,
    reviewCount: 67,
    image: "/venues/volleyball-1.jpg",
    isOpen: true,
    hasLighting: false,
    isFree: true,
    workingHours: "07:00 - 22:00",
    coordinates: { lat: 55.7911, lng: 37.6715 },
  },
  {
    id: "6",
    name: "Скейт-парк Коломенское",
    description: "Современный скейт-парк с рампами, рейлами и боулом.",
    city: "Москва",
    address: "пр-т Андропова, 39",
    sportType: "Скейтбординг",
    surface: "Бетон",
    rating: 4.7,
    reviewCount: 145,
    image: "/venues/skate-1.jpg",
    isOpen: true,
    hasLighting: true,
    isFree: true,
    workingHours: "09:00 - 21:00",
    coordinates: { lat: 55.6673, lng: 37.6711 },
  },
  {
    id: "7",
    name: "Хоккейная коробка Измайлово",
    description: "Крытая хоккейная коробка с искусственным льдом круглый год.",
    city: "Москва",
    address: "Измайловский проезд, 10",
    sportType: "Хоккей",
    surface: "Лёд",
    rating: 4.4,
    reviewCount: 78,
    image: "/venues/hockey-1.jpg",
    isOpen: true,
    hasLighting: true,
    isFree: false,
    workingHours: "10:00 - 22:00",
    coordinates: { lat: 55.7887, lng: 37.7479 },
  },
  {
    id: "8",
    name: "Беговые дорожки Царицыно",
    description: "Профессиональные беговые дорожки с разметкой на 400м.",
    city: "Москва",
    address: "ул. Дольская, 1",
    sportType: "Бег",
    surface: "Тартан",
    rating: 4.6,
    reviewCount: 92,
    image: "/venues/running-1.jpg",
    isOpen: true,
    hasLighting: true,
    isFree: true,
    workingHours: "06:00 - 23:00",
    coordinates: { lat: 55.6167, lng: 37.6861 },
  },
];

export const cities = [
  "Москва",
  "Санкт-Петербург",
  "Казань",
  "Новосибирск",
  "Екатеринбург",
];
export const sportTypes = [
  "Баскетбол",
  "Футбол",
  "Теннис",
  "Волейбол",
  "Воркаут",
  "Скейтбординг",
  "Хоккей",
  "Бег",
];
export const surfaces = [
  "Резиновое",
  "Искусственный газон",
  "Грунт",
  "Песок",
  "Бетон",
  "Лёд",
  "Тартан",
  "Асфальт",
];
