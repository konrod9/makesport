import { VenueDetails } from "@/shared/components/venues/venue-details";

export default async function VenuePage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  return <VenueDetails id={id} />;
}
