import { MapLoader } from "@/shared/components/map/map-loader";
import { Metadata } from "next";

export const metadata: Metadata = {
  title: "Карта площадок — MakeSport",
};

export default function MapPage() {
  return <MapLoader />;
}
