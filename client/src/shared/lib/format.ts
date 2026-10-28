export function formatDistance(
  distanceMeters: number | null | undefined,
): string | null {
  if (distanceMeters == null || !Number.isFinite(distanceMeters)) {
    return null;
  }

  if (distanceMeters < 1000) {
    return `${Math.round(distanceMeters)} м`;
  }

  const kilometers = distanceMeters / 1000;

  return `${new Intl.NumberFormat("ru-RU", {
    maximumFractionDigits: 1,
  }).format(kilometers)} км`;
}
