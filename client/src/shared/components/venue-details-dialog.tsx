"use client";

import { useEffect } from "react";
import { createPortal } from "react-dom";
import {
  Star,
  MapPin,
  Clock,
  Lightbulb,
  CircleDollarSign,
  Building2,
  Layers,
  Navigation,
  ArrowRight,
  X,
} from "lucide-react";
import { ImageGallery } from "@/shared/components/image-gallery";
import { Venue } from "@/entities/venues/types";
import { formatDistance } from "@/shared/lib/format";
import { useRouter } from "next/navigation";

interface VenueDetailsDialogProps {
  venue: Venue | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function VenueDetailsDialog({
  venue,
  open,
  onOpenChange,
}: VenueDetailsDialogProps) {
  const router = useRouter();

  // Закрытие по Esc
  useEffect(() => {
    if (!open) return;
    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") onOpenChange(false);
    };
    document.addEventListener("keydown", onKeyDown);
    return () => document.removeEventListener("keydown", onKeyDown);
  }, [open, onOpenChange]);

  // Блокировка скролла фона
  useEffect(() => {
    if (!open) return;
    const original = document.body.style.overflow;
    document.body.style.overflow = "hidden";
    return () => {
      document.body.style.overflow = original;
    };
  }, [open]);

  if (!venue || !open) return null;

  const distance = formatDistance(venue.distanceMeters);

  return createPortal(
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-6">
      {/* Оверлей */}
      <div
        className="absolute inset-0 bg-ink/40 backdrop-blur-sm"
        onClick={() => onOpenChange(false)}
        aria-hidden="true"
      />

      {/* Модалка */}
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby="venue-details-title"
        className="relative w-full max-w-lg animate-fade-up overflow-hidden rounded-4xl border border-ink-200/70 bg-white shadow-card"
      >
        {/* Кнопка закрытия — поверх всего, не скроллится */}
        <button
          type="button"
          onClick={() => onOpenChange(false)}
          className="absolute right-4 top-4 z-10 grid size-9 place-items-center rounded-full bg-white/90 text-ink shadow-soft backdrop-blur-sm transition-colors hover:bg-white"
          aria-label="Закрыть"
        >
          <X className="size-4" />
        </button>

        {/* Скроллируемая часть */}
        <div className="max-h-[90vh] overflow-y-auto">
          <ImageGallery
            images={venue.images}
            title={venue.title}
            sportType={venue.sportType}
            isOpen={venue.isOpen}
            isFree={venue.isFree}
          />

          <div className="space-y-5 p-6 sm:p-8">
            {/* Заголовок + рейтинг */}
            <div className="flex items-start justify-between gap-3">
              <h2
                id="venue-details-title"
                className="text-balance font-display text-xl font-bold leading-tight text-ink"
              >
                {venue.title}
              </h2>
              <div className="flex shrink-0 items-center gap-1 rounded-full bg-lime px-2.5 py-1">
                <Star className="size-3.5 fill-ink text-ink" />
                <span className="text-sm font-bold text-ink">
                  {venue.rating}
                </span>
                <span className="text-[11px] font-medium text-ink-500">
                  ({venue.reviewCount})
                </span>
              </div>
            </div>

            {/* Описание */}
            <p className="text-sm leading-relaxed text-ink-500">
              {venue.description}
            </p>

            {/* Ключевые факты */}
            <div className="grid grid-cols-2 gap-3">
              <FactCard
                icon={<Building2 className="size-4" />}
                label="Вид спорта"
                value={venue.sportType}
              />
              <FactCard
                icon={<Layers className="size-4" />}
                label="Покрытие"
                value={venue.surface}
              />
              <FactCard
                icon={<Clock className="size-4" />}
                label="Время работы"
                value={venue.workingHours}
              />
              <FactCard
                icon={<CircleDollarSign className="size-4" />}
                label="Стоимость"
                value={venue.isFree ? "Бесплатно" : "Платно"}
              />
            </div>

            {/* Освещение */}
            <div className="flex items-center gap-3 rounded-2xl bg-cream p-4">
              <span
                className={`grid size-9 shrink-0 place-items-center rounded-xl ${
                  venue.hasLighting
                    ? "bg-lime text-ink"
                    : "bg-ink-100 text-ink-400"
                }`}
              >
                <Lightbulb className="size-4" />
              </span>
              <p className="text-sm font-medium text-ink">
                {venue.hasLighting
                  ? "Есть освещение для вечерних игр"
                  : "Освещение отсутствует"}
              </p>
            </div>

            {/* Адрес */}
            <div className="flex items-start gap-3 rounded-2xl border border-ink-100 p-4">
              <span className="grid size-9 shrink-0 place-items-center rounded-xl bg-lime/20 text-lime-600">
                <MapPin className="size-4" />
              </span>
              <div>
                <p className="text-[10px] font-semibold uppercase tracking-wider text-ink-400">
                  Адрес
                </p>
                <p className="mt-0.5 text-sm font-semibold text-ink">
                  {venue.address.street}
                </p>
                <p className="text-sm text-ink-500">{venue.address.city}</p>
              </div>
            </div>

            {/* Расстояние */}
            {distance && (
              <div className="flex items-center gap-3 rounded-2xl bg-cream p-4">
                <span className="grid size-9 shrink-0 place-items-center rounded-xl bg-white text-ink">
                  <Navigation className="size-4" />
                </span>
                <div>
                  <p className="text-[10px] font-semibold uppercase tracking-wider text-ink-400">
                    Расстояние
                  </p>
                  <p className="mt-0.5 text-sm font-semibold text-ink">
                    {distance}
                  </p>
                </div>
              </div>
            )}

            {/* Кнопка «Подробнее» */}
            <button
              type="button"
              onClick={() => router.push("/venues/" + venue.id)}
              className="group inline-flex w-full items-center justify-center gap-2 rounded-full bg-lime px-6 py-3.5 text-sm font-bold text-ink transition-all hover:shadow-lime"
            >
              Подробнее
              <ArrowRight className="size-4 transition-transform group-hover:translate-x-0.5" />
            </button>
          </div>
        </div>
      </div>
    </div>,
    document.body,
  );
}

function FactCard({
  icon,
  label,
  value,
}: {
  icon: React.ReactNode;
  label: string;
  value: string;
}) {
  return (
    <div className="flex items-center gap-3 rounded-2xl bg-cream p-3.5">
      <span className="grid size-9 shrink-0 place-items-center rounded-xl bg-lime text-ink">
        {icon}
      </span>
      <div className="min-w-0">
        <p className="text-[10px] font-semibold uppercase tracking-wider text-ink-400">
          {label}
        </p>
        <p className="mt-0.5 truncate text-sm font-semibold text-ink">
          {value}
        </p>
      </div>
    </div>
  );
}
