"use client";

import { AddressDto, CoordinatesDto } from "@/entities/venues/types";
import {
  YMaps,
  Map,
  Placemark,
  useYMaps,
  SearchControl,
} from "@pbe/react-yandex-maps";
import { useCallback, useState } from "react";

type GeocoderGeoObject = {
  getAddressLine(): string;
  getLocalities(): string[];
  getThoroughfare(): string;
  getPremiseNumber(): string;
  properties: {
    get(key: string): unknown;
  };
};

type MapPickerProps = {
  onCoordinatesChange?: (coords: CoordinatesDto) => void;
  onAddressChange?: (address: AddressDto) => void;
  className?: string;
};

type YandexMapEvent = {
  get(key: string): unknown;
};

type YandexPlacemark = {
  geometry: {
    getCoordinates(): [number, number];
  };
};

type MetaDataProperty = {
  GeocoderMetaData?: {
    Address?: {
      Components?: Array<{ kind: string; name: string }>;
    };
  };
};

function MapInner({ onCoordinatesChange, onAddressChange }: MapPickerProps) {
  const ymaps = useYMaps(["geocode"]);
  const [coords, setCoords] = useState<[number, number] | null>(null);

  const geocodeAndUpdate = useCallback(
    async (newCoords: [number, number]) => {
      onCoordinatesChange?.({
        latitude: newCoords[0],
        longitude: newCoords[1],
      });

      if (!ymaps) return;

      const result = await ymaps.geocode(newCoords, {
        kind: "house",
        results: 1,
      });
      const geo = result.geoObjects.get(0) as unknown as GeocoderGeoObject;
      if (!geo) return;

      let city = "";
      let street = "";
      let building = "";

      const metaData = geo.properties.get(
        "metaDataProperty",
      ) as MetaDataProperty;
      const addressComponents:
        | Array<{ kind: string; name: string }>
        | undefined = metaData?.GeocoderMetaData?.Address?.Components;

      if (addressComponents) {
        for (const c of addressComponents) {
          if (c.kind === "locality") city = c.name;
          if (c.kind === "street") street = c.name;
          if (c.kind === "house") building = c.name;
        }
      }

      if (!city) city = geo.getLocalities()?.[0] ?? "";
      if (!street) street = geo.getThoroughfare() ?? "";
      if (!building) building = geo.getPremiseNumber() ?? "";

      const streetFull = street + (building ? `, ${building}` : "");

      onAddressChange?.({
        city,
        street: streetFull || street,
        building,
        fullName: geo.getAddressLine() ?? "",
      });
    },
    [ymaps, onCoordinatesChange, onAddressChange],
  );

  const handleClick = useCallback(
    (e: YandexMapEvent) => {
      const newCoords = e.get("coords") as [number, number];
      setCoords(newCoords);
      geocodeAndUpdate(newCoords);
    },
    [geocodeAndUpdate],
  );

  const handleDragEnd = useCallback(
    (e: YandexMapEvent) => {
      const target = e.get("target") as YandexPlacemark;
      const newCoords = target.geometry.getCoordinates();
      setCoords(newCoords);
      geocodeAndUpdate(newCoords);
    },
    [geocodeAndUpdate],
  );

  return (
    <Map
      defaultState={{ center: [55.75, 37.57], zoom: 9 }}
      className="w-full h-full"
      onClick={handleClick}
    >
      {coords && (
        <Placemark
          geometry={coords}
          options={{ preset: "islands#redDotIcon", draggable: true }}
          onDragEnd={handleDragEnd}
        />
      )}
      <SearchControl options={{ float: "right" }} />
    </Map>
  );
}

export function MapPicker(props: MapPickerProps) {
  return (
    <YMaps
      query={{
        apikey: process.env.NEXT_PUBLIC_YMAPS_API_KEY ?? "",
      }}
    >
      <MapInner {...props} />
    </YMaps>
  );
}
