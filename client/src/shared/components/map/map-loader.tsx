"use client";

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

export function MapLoader() {
  return <MapView />;
}
