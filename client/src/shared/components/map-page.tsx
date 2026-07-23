"use client";

import { Venue } from "@/entities/venues/types";
import { YMaps, Map, Placemark } from "@pbe/react-yandex-maps";

interface VenueMapProps {
  venues: Venue[];
  selectedVenue: Venue | null;
  onSelectVenue: (venue: Venue | null) => void;
  onShowDetails?: (venue: Venue) => void;
}

export function VenuesMap({
  venues,
  onSelectVenue,
  onShowDetails,
}: VenueMapProps) {
  return (
    <YMaps
      query={{
        apikey: process.env.NEXT_PUBLIC_YMAPS_API_KEY ?? "",
      }}
    >
      <Map
        defaultState={{ center: [55.75, 37.57], zoom: 9 }}
        className="w-full h-full"
      >
        {venues.map((venue) => (
          <Placemark
            key={venue.id}
            geometry={[venue.coordinates.latitude, venue.coordinates.longitude]}
            properties={{
              hintContent: venue.title,
              balloonContentBody: venue.title,
            }}
            options={{
              preset: "islands#blueDotIcon",
            }}
            onClick={() => {
              onSelectVenue(venue);
              onShowDetails?.(venue);
            }}
          />
        ))}
      </Map>
    </YMaps>
  );
}
