"use client";

// import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
// import { Icon } from "leaflet";
import { Star, MapPin, Clock, X } from "lucide-react";
import { Badge } from "@/shared/components/ui/badge";
import { Button } from "@/shared/components/ui/button";

import "leaflet/dist/leaflet.css";
import { Venue } from "@/entities/venues/types";

interface VenueMapProps {
  venues: Venue[];
  selectedVenue: Venue | null;
  onSelectVenue: (venue: Venue | null) => void;
  onShowDetails?: (venue: Venue) => void;
}

const getSportEmoji = (sportType: string) => {
  switch (sportType) {
    case "Баскетбол":
      return "🏀";
    case "Футбол":
      return "⚽";
    case "Теннис":
      return "🎾";
    case "Волейбол":
      return "🏐";
    case "Воркаут":
      return "💪";
    case "Скейтбординг":
      return "🛹";
    case "Хоккей":
      return "🏒";
    case "Бег":
      return "🏃";
    default:
      return "📍";
  }
};

const createCustomIcon = () => {
  return new Icon({
    iconUrl:
      "data:image/svg+xml;base64," +
      btoa(`
      <svg xmlns="http://www.w3.org/2000/svg" width="32" height="40" viewBox="0 0 32 40">
        <path d="M16 0C7.163 0 0 7.163 0 16c0 12 16 24 16 24s16-12 16-24C32 7.163 24.837 0 16 0z" fill="#ea580c"/>
        <circle cx="16" cy="14" r="6" fill="#1c1917"/>
      </svg>
    `),
    iconSize: [32, 40],
    iconAnchor: [16, 40],
    popupAnchor: [0, -40],
  });
};

export function VenueMap({
  venues,
  selectedVenue,
  onSelectVenue,
  onShowDetails,
}: VenueMapProps) {
  const center =
    venues.length > 0
      ? { lat: venues[0].coordinates.lat, lng: venues[0].coordinates.lng }
      : { lat: 55.7558, lng: 37.6173 };

  const customIcon = createCustomIcon();

  return (
    <div className="relative h-full w-full">
      <MapContainer
        center={[center.lat, center.lng]}
        zoom={11}
        className="h-full w-full rounded-lg"
        style={{ background: "#1c1917" }}
      >
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
          url="https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png"
        />
        {venues.map((venue) => (
          <Marker
            key={venue.id}
            position={[venue.coordinates.lat, venue.coordinates.lng]}
            icon={customIcon}
            eventHandlers={{
              click: () => onSelectVenue(venue),
            }}
          >
            <Popup className="venue-popup">
              <div className="text-sm font-medium">{venue.name}</div>
            </Popup>
          </Marker>
        ))}
      </MapContainer>

      {/* Selected Venue Card Overlay */}
      {selectedVenue && (
        <div className="absolute bottom-4 left-4 right-4 md:left-auto md:right-4 md:w-96 z-[1000]">
          <div className="bg-card border border-border rounded-lg shadow-xl overflow-hidden">
            <div className="relative">
              <div className="h-32 bg-gradient-to-br from-muted to-secondary flex items-center justify-center">
                <span className="text-5xl">
                  {getSportEmoji(selectedVenue.sportType)}
                </span>
              </div>
              <Button
                variant="ghost"
                size="icon"
                className="absolute top-2 right-2 bg-background/80 hover:bg-background"
                onClick={() => onSelectVenue(null)}
              >
                <X className="h-4 w-4" />
              </Button>
              <div className="absolute top-2 left-2 flex gap-2">
                <Badge
                  variant={selectedVenue.isOpen ? "default" : "secondary"}
                  className={
                    selectedVenue.isOpen
                      ? "bg-green-600 hover:bg-green-600 text-foreground"
                      : ""
                  }
                >
                  {selectedVenue.isOpen ? "Открыто" : "Закрыто"}
                </Badge>
                {selectedVenue.isFree && (
                  <Badge
                    variant="secondary"
                    className="bg-primary/20 text-primary border-0"
                  >
                    Бесплатно
                  </Badge>
                )}
              </div>
            </div>
            <div className="p-4">
              <div className="flex items-start justify-between gap-2 mb-2">
                <h3 className="font-semibold text-foreground line-clamp-1">
                  {selectedVenue.name}
                </h3>
                <div className="flex items-center gap-1 shrink-0">
                  <Star className="h-4 w-4 fill-primary text-primary" />
                  <span className="text-sm font-medium text-foreground">
                    {selectedVenue.rating}
                  </span>
                </div>
              </div>
              <p className="text-sm text-muted-foreground line-clamp-2 mb-3">
                {selectedVenue.description}
              </p>
              <div className="flex flex-wrap gap-2 mb-3">
                <Badge variant="outline" className="text-xs">
                  {selectedVenue.sportType}
                </Badge>
                <Badge variant="outline" className="text-xs">
                  {selectedVenue.surface}
                </Badge>
              </div>
              <div className="flex flex-col gap-1.5 text-sm text-muted-foreground">
                <div className="flex items-center gap-2">
                  <MapPin className="h-3.5 w-3.5 text-primary" />
                  <span className="line-clamp-1">
                    {selectedVenue.address}, {selectedVenue.city}
                  </span>
                </div>
                <div className="flex items-center gap-2">
                  <Clock className="h-3.5 w-3.5 text-primary" />
                  <span>{selectedVenue.workingHours}</span>
                </div>
              </div>
              {onShowDetails && (
                <Button
                  className="w-full mt-4"
                  onClick={() => onShowDetails(selectedVenue)}
                >
                  Подробнее
                </Button>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
