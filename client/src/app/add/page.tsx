import { AddVenueForm } from "@/shared/components/add-venue-form";
import { Metadata } from "next";

export const metadata: Metadata = {
  title: "Добавить площадку - MakeSport",
};

export default function AddPage() {
  return <AddVenueForm />;
}
