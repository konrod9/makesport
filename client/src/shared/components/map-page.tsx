"use client";

import { YMaps, Map, Placemark } from "@pbe/react-yandex-maps";
import { Venue } from "../lib/data";

interface VenueMapProps {
  venues: Venue[];
  selectedVenue: Venue | null;
  onSelectVenue: (venue: Venue | null) => void;
  onShowDetails?: (venue: Venue) => void;
}

export function VenuesMap({
  venues,
  selectedVenue,
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
            geometry={[venue.coordinates.lat, venue.coordinates.lng]}
            properties={{
              hintContent: venue.name,
              balloonContentBody: venue.name,
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
