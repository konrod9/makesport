"use client";

import Image from "next/image";
import Link from "next/link";
import { notFound } from "next/navigation";
import { useRef, useState, type KeyboardEvent, type TouchEvent } from "react";
import {
  ArrowLeft,
  CalendarClock,
  Check,
  ChevronLeft,
  ChevronRight,
  Clock,
  MapPin,
  Share2,
  Star,
  Sun,
  Wallet,
  X,
} from "lucide-react";
import { Badge } from "@/shared/components/ui/badge";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
} from "@/shared/components/ui/dialog";
import { useGetVenueById } from "@/features/venues/model/use-get-venue-by-id";
import { formatDistance } from "@/shared/lib/format";
import { cn } from "@/shared/lib/utils";

export function VenueDetails({ id }: { id: string }) {
  const { data, isPending, isError } = useGetVenueById(id);

  const [currentIndex, setCurrentIndex] = useState(0);
  const [prevVenueId, setPrevVenueId] = useState<string | null>(null);
  const [isLightboxOpen, setIsLightboxOpen] = useState(false);
  const touchStart = useRef({ x: 0, y: 0 });
  const touchEnd = useRef({ x: 0, y: 0 });
  const didSwipe = useRef(false);

  if (isPending) return null; //<VenuePageSkeleton />;
  if (isError || !data?.result) notFound();

  const venue = data.result;
  const images = venue.images.filter((image) => image.url);
  const hasMultiple = images.length > 1;
  const safeIndex =
    images.length > 0 ? Math.min(currentIndex, images.length - 1) : 0;

  if (prevVenueId !== venue.id) {
    setPrevVenueId(venue.id);
    setCurrentIndex(0);
    setIsLightboxOpen(false);
  }

  const goTo = (index: number) => {
    if (images.length === 0) return;
    setCurrentIndex(((index % images.length) + images.length) % images.length);
  };

  const goNext = () => goTo(safeIndex + 1);
  const goPrev = () => goTo(safeIndex - 1);

  const openLightbox = () => {
    if (images.length === 0) return;
    setIsLightboxOpen(true);
  };

  const handleImageClick = () => {
    if (didSwipe.current) {
      didSwipe.current = false;
      return;
    }
    openLightbox();
  };

  const handleArrowKeys = (event: KeyboardEvent) => {
    if (!hasMultiple) return;
    if (event.key === "ArrowLeft") {
      event.preventDefault();
      goPrev();
    }
    if (event.key === "ArrowRight") {
      event.preventDefault();
      goNext();
    }
  };

  const handleCarouselKeyDown = (event: KeyboardEvent) => {
    if (event.target !== event.currentTarget) return;

    if (event.key === "ArrowLeft" || event.key === "ArrowRight") {
      handleArrowKeys(event);
      return;
    }

    if (event.key === "Enter" || event.key === " ") {
      event.preventDefault();
      openLightbox();
    }
  };

  const handleTouchStart = (event: TouchEvent) => {
    didSwipe.current = false;
    touchStart.current = {
      x: event.touches[0].clientX,
      y: event.touches[0].clientY,
    };
  };

  const handleTouchMove = (event: TouchEvent) => {
    touchEnd.current = {
      x: event.touches[0].clientX,
      y: event.touches[0].clientY,
    };
  };

  const handleTouchEnd = () => {
    if (!hasMultiple) return;

    const deltaX = touchStart.current.x - touchEnd.current.x;
    const deltaY = touchStart.current.y - touchEnd.current.y;

    if (Math.abs(deltaX) > 50 && Math.abs(deltaX) > Math.abs(deltaY)) {
      didSwipe.current = true;
      if (deltaX > 0) goNext();
      else goPrev();
    }

    touchStart.current = { x: 0, y: 0 };
    touchEnd.current = { x: 0, y: 0 };
  };

  // const similar = venues
  //   .filter((v) => v.id !== venue.id && v.city === venue.city)
  //   .slice(0, 3);

  return (
    <div className="mx-auto max-w-7xl px-5 py-8 sm:px-8 sm:py-12">
      <Link
        href="/"
        className="inline-flex items-center gap-2 text-sm font-semibold text-ink-400 transition-colors hover:text-ink"
      >
        <ArrowLeft className="size-4" /> Все площадки
      </Link>

      <div className="mt-6 overflow-hidden rounded-[2.5rem] border border-ink-200/70 bg-white shadow-card">
        <div
          className="group relative aspect-[16/9] max-h-[520px] w-full cursor-zoom-in touch-pan-y overflow-hidden bg-ink-100 sm:aspect-[21/9] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-lime"
          role={hasMultiple ? "region" : undefined}
          aria-roledescription={hasMultiple ? "carousel" : undefined}
          aria-label={hasMultiple ? "Фотографии площадки" : undefined}
          tabIndex={hasMultiple ? 0 : undefined}
          onClick={handleImageClick}
          onKeyDown={handleCarouselKeyDown}
          onTouchStart={handleTouchStart}
          onTouchMove={handleTouchMove}
          onTouchEnd={handleTouchEnd}
        >
          {images.length > 0 ? (
            <div
              className="flex h-full w-full transition-transform duration-500 ease-out"
              style={{ transform: `translateX(-${safeIndex * 100}%)` }}
            >
              {images.map((image, index) => (
                <div
                  key={image.id}
                  className="relative h-full w-full shrink-0"
                  aria-hidden={index !== safeIndex}
                >
                  <Image
                    src={image.url}
                    alt={`${venue.title} — фото ${index + 1}`}
                    fill
                    unoptimized
                    preload={index === 0}
                    loading={index < 2 ? "eager" : "lazy"}
                    draggable={false}
                    sizes="(max-width: 1280px) 100vw, 1280px"
                    className="object-cover"
                  />
                </div>
              ))}
            </div>
          ) : (
            <div className="absolute inset-0 grid place-items-center bg-gradient-to-br from-ink-100 to-cream text-ink-300">
              <MapPin className="size-10" />
            </div>
          )}

          <div className="pointer-events-none absolute inset-0 bg-gradient-to-t from-ink/80 via-ink/10 to-transparent" />

          <div className="absolute left-5 top-5 hidden flex-wrap gap-2 sm:flex">
            <Badge tone="lime">{venue.isFree ? "Бесплатно" : "Платно"}</Badge>
          </div>

          <button
            type="button"
            onClick={(event) => event.stopPropagation()}
            className="absolute right-5 top-5 hidden size-11 place-items-center rounded-full bg-cream/90 text-ink backdrop-blur transition-colors hover:bg-cream sm:grid"
          >
            <Share2 className="size-4" />
          </button>

          <div className="absolute inset-x-5 bottom-5 sm:inset-x-8 sm:bottom-8">
            <div className="hidden flex-wrap gap-2 sm:flex">
              {/* {venue.sports.map((s) => (
                <span
                  key={s}
                  className="rounded-full bg-cream/95 px-3 py-1 text-xs font-semibold text-ink backdrop-blur"
                >
                  {s}
                </span>
              ))} */}
              <span className="rounded-full bg-cream/95 px-3 py-1 text-xs font-semibold text-ink backdrop-blur">
                {venue.sportType}
              </span>
            </div>
            <h1 className="mt-3 hidden font-display text-3xl font-bold leading-tight text-cream sm:block sm:text-4xl lg:text-5xl">
              {venue.title}
            </h1>
            <p className="mt-3 flex flex-wrap items-center gap-x-5 gap-y-2 text-sm text-cream/75">
              <span className="inline-flex items-center gap-1.5">
                <MapPin className="size-4" /> {venue.address.city},{" "}
                {venue.address.street}
              </span>
              <span className="hidden items-center gap-1.5 sm:inline-flex">
                <Star className="size-4 fill-lime text-lime" /> {venue.rating} ·{" "}
                {venue.reviewCount} отзывов
              </span>
              {venue.distanceMeters && (
                <span className="hidden sm:inline">
                  {formatDistance(venue.distanceMeters)} от вас
                </span>
              )}
            </p>
          </div>

          {hasMultiple && (
            <>
              <button
                type="button"
                onClick={(event) => {
                  event.stopPropagation();
                  goPrev();
                }}
                aria-label="Предыдущее фото"
                className="absolute left-3 top-1/2 z-20 grid size-10 -translate-y-1/2 place-items-center rounded-full bg-cream/85 text-ink shadow-card backdrop-blur transition-colors hover:bg-cream sm:left-5 sm:size-11"
              >
                <ChevronLeft className="size-5" />
              </button>
              <button
                type="button"
                onClick={(event) => {
                  event.stopPropagation();
                  goNext();
                }}
                aria-label="Следующее фото"
                className="absolute right-3 top-1/2 z-20 grid size-10 -translate-y-1/2 place-items-center rounded-full bg-cream/85 text-ink shadow-card backdrop-blur transition-colors hover:bg-cream sm:right-5 sm:size-11"
              >
                <ChevronRight className="size-5" />
              </button>

              <span
                aria-live="polite"
                className="absolute right-4 top-4 z-20 rounded-full bg-ink/55 px-2.5 py-1 text-[11px] font-semibold tabular-nums text-cream backdrop-blur sm:right-20 sm:top-5"
              >
                {safeIndex + 1} / {images.length}
              </span>

              <div className="absolute bottom-2 left-1/2 z-20 hidden -translate-x-1/2 items-center gap-1.5 sm:flex">
                {images.map((image, index) => (
                  <button
                    key={image.id}
                    type="button"
                    onClick={(event) => {
                      event.stopPropagation();
                      goTo(index);
                    }}
                    aria-label={`Фото ${index + 1}`}
                    aria-current={index === safeIndex ? "true" : undefined}
                    className={cn(
                      "h-1.5 rounded-full transition-all",
                      index === safeIndex
                        ? "w-6 bg-cream"
                        : "w-1.5 bg-cream/50 hover:bg-cream/80",
                    )}
                  />
                ))}
              </div>
            </>
          )}
        </div>

        <div className="px-6 pt-6 sm:hidden">
          <div className="flex flex-wrap gap-2">
            <Badge tone="lime">{venue.isFree ? "Бесплатно" : "Платно"}</Badge>
            <span className="rounded-full bg-ink-50 px-3 py-1 text-xs font-semibold text-ink-600">
              {venue.sportType}
            </span>
          </div>
          <h1 className="mt-3 font-display text-3xl font-bold leading-tight text-ink">
            {venue.title}
          </h1>
          <p className="mt-2 flex flex-wrap items-center gap-x-4 gap-y-2 text-sm text-ink-500">
            <span className="inline-flex items-center gap-1.5">
              <Star className="size-4 fill-lime-500 text-lime-500" />{" "}
              {venue.rating} · {venue.reviewCount} отзывов
            </span>
            {venue.distanceMeters && (
              <span>{formatDistance(venue.distanceMeters)} от вас</span>
            )}
          </p>
        </div>

        <div className="grid gap-8 p-6 sm:p-8 lg:grid-cols-[1.5fr_1fr] lg:p-10">
          <div>
            <h2 className="font-display text-xl font-semibold">О площадке</h2>
            <p className="mt-3 leading-relaxed text-ink-500">
              {venue.description}
            </p>

            <div className="mt-8 grid gap-3 sm:grid-cols-2">
              {[
                {
                  icon: Clock,
                  label: "Часы работы",
                  value: venue.workingHours,
                },
                { icon: Wallet, label: "Покрытие", value: venue.surface },
                {
                  icon: Sun,
                  label: "Освещение",
                  value: venue.hasLighting ? "Есть" : "Нет",
                },
                {
                  icon: CalendarClock,
                  label: "Стоимость",
                  value: venue.isFree ? "Бесплатно" : "По тарифу",
                },
              ].map((item) => (
                <div
                  key={item.label}
                  className="flex items-center gap-3 rounded-2xl border border-ink-100 bg-cream/60 p-4"
                >
                  <span className="grid size-10 place-items-center rounded-xl bg-white text-ink">
                    <item.icon className="size-4" />
                  </span>
                  <div>
                    <p className="text-[11px] font-semibold uppercase tracking-wider text-ink-400">
                      {item.label}
                    </p>
                    <p className="text-sm font-semibold text-ink">
                      {item.value}
                    </p>
                  </div>
                </div>
              ))}
            </div>

            {/* <div className="mt-8">
              <h2 className="font-display text-xl font-semibold">Удобства</h2>
              <div className="mt-3 flex flex-wrap gap-2">
                {[
                  "Освещение",
                  venue.isFree ? "Бесплатный вход" : "Оплата картой",
                  "Рядом парковка",
                  "Тренировки круглый год",
                  "Подходит для новичков",
                ].map((a) => (
                  <span
                    key={a}
                    className="inline-flex items-center gap-1.5 rounded-full bg-ink-50 px-3 py-1.5 text-xs font-semibold text-ink-600"
                  >
                    <Check className="size-3.5 text-lime-600" /> {a}
                  </span>
                ))}
              </div>
            </div> */}

            {/* <div className="mt-8">
              <h2 className="font-display text-xl font-semibold">Отзывы</h2>
              <div className="mt-4 space-y-3">
                {[
                  {
                    name: "Дмитрий",
                    text: "Отличное место, покрытие ровное, вечером хорошо освещено. Приходим играть по вторникам.",
                    rating: 5,
                  },
                  {
                    name: "Ольга",
                    text: "Удобно добираться, есть где оставить велосипед. Иногда бывает людно после работы.",
                    rating: 4,
                  },
                ].map((r) => (
                  <div
                    key={r.name}
                    className="rounded-2xl border border-ink-100 bg-white p-5"
                  >
                    <div className="flex items-center justify-between">
                      <p className="text-sm font-semibold text-ink">{r.name}</p>
                      <span className="flex items-center gap-0.5">
                        {Array.from({ length: 5 }).map((_, i) => (
                          <Star
                            key={i}
                            className={
                              i < r.rating
                                ? "size-3.5 fill-lime-500 text-lime-500"
                                : "size-3.5 text-ink-200"
                            }
                          />
                        ))}
                      </span>
                    </div>
                    <p className="mt-2 text-sm leading-relaxed text-ink-500">
                      {r.text}
                    </p>
                  </div>
                ))}
              </div>
            </div>*/}
          </div>

          <aside className="space-y-4">
            <div className="rounded-3xl bg-ink p-6 text-cream">
              <p className="text-xs font-semibold uppercase tracking-wider text-cream/50">
                Добраться
              </p>
              <p className="mt-2 font-display text-lg font-semibold">
                {venue.address.city}
              </p>
              <p className="mt-1 text-sm text-cream/60">
                {venue.address.street}
              </p>
              <a
                href={`https://yandex.ru/maps/?pt=${venue.coordinates.longitude},${venue.coordinates.latitude}&z=16`}
                target="_blank"
                rel="noreferrer"
                className="mt-5 flex items-center justify-center gap-2 rounded-full bg-lime py-3 text-sm font-bold text-ink transition-transform hover:-translate-y-0.5"
              >
                <MapPin className="size-4" /> Построить маршрут
              </a>
              {venue.distanceMeters && (
                <p className="mt-4 text-center text-[11px] text-cream/40">
                  {formatDistance(venue.distanceMeters)} от вас
                </p>
              )}
            </div>

            {/* <div className="rounded-3xl border border-ink-200/70 bg-white p-6">
              <p className="text-xs font-semibold uppercase tracking-wider text-ink-400">
                Похожие площадки
              </p>
              <div className="mt-4 space-y-3">
                {similar.map((v) => (
                  <Link
                    key={v.id}
                    href={`/venue/${v.id}`}
                    className="group flex items-center gap-3 rounded-2xl p-2 transition-colors hover:bg-cream"
                  >
                    <div className="relative size-14 shrink-0 overflow-hidden rounded-xl">
                      <Image
                        src={v.image}
                        alt={v.name}
                        fill
                        sizes="56px"
                        className="object-cover"
                      />
                    </div>
                    <div className="min-w-0">
                      <p className="line-clamp-1 text-[13px] font-semibold text-ink group-hover:text-lime-600">
                        {v.name}
                      </p>
                      <p className="text-[11px] text-ink-400">
                        ★ {v.rating} · {v.distance}
                      </p>
                    </div>
                    <ArrowUpRight className="ml-auto size-4 shrink-0 text-ink-200 group-hover:text-ink" />
                  </Link>
                ))}
              </div>
            </div> */}
          </aside>
        </div>
      </div>

      {images.length > 0 && (
        <Dialog open={isLightboxOpen} onOpenChange={setIsLightboxOpen}>
          <DialogContent
            showCloseButton={false}
            className="fixed top-0 right-0 bottom-0 left-0 z-50 grid h-dvh w-screen max-w-none translate-x-0 translate-y-0 gap-0 overflow-hidden rounded-none border-0 bg-ink/95 p-0 shadow-none sm:max-w-none"
            onKeyDown={handleArrowKeys}
            onTouchStart={handleTouchStart}
            onTouchMove={handleTouchMove}
            onTouchEnd={handleTouchEnd}
          >
            <DialogTitle className="sr-only">
              {venue.title} — фотографии
            </DialogTitle>
            <DialogDescription className="sr-only">
              Просмотр фотографий площадки. Используйте стрелки влево и вправо
              для переключения.
            </DialogDescription>

            <div className="absolute inset-0 px-4 pb-16 pt-20 sm:px-16 sm:py-20">
              <div className="relative h-full w-full">
                <Image
                  key={images[safeIndex].id}
                  src={images[safeIndex].url}
                  alt={`${venue.title} — фото ${safeIndex + 1}`}
                  fill
                  unoptimized
                  sizes="100vw"
                  draggable={false}
                  className="object-contain"
                />
              </div>
            </div>

            <div className="absolute inset-x-0 top-0 z-20 flex items-center justify-between p-4 sm:p-6">
              <span className="rounded-full bg-cream/15 px-3 py-1.5 text-xs font-semibold tabular-nums text-cream backdrop-blur">
                {safeIndex + 1} / {images.length}
              </span>
              <button
                type="button"
                onClick={() => setIsLightboxOpen(false)}
                aria-label="Закрыть"
                className="grid size-10 place-items-center rounded-full bg-cream/15 text-cream backdrop-blur transition-colors hover:bg-cream/25 sm:size-11"
              >
                <X className="size-5" />
              </button>
            </div>

            {hasMultiple && (
              <>
                <button
                  type="button"
                  onClick={goPrev}
                  aria-label="Предыдущее фото"
                  className="absolute left-3 top-1/2 z-20 grid size-11 -translate-y-1/2 place-items-center rounded-full bg-cream/15 text-cream backdrop-blur transition-colors hover:bg-cream/25 sm:left-6 sm:size-12"
                >
                  <ChevronLeft className="size-6" />
                </button>
                <button
                  type="button"
                  onClick={goNext}
                  aria-label="Следующее фото"
                  className="absolute right-3 top-1/2 z-20 grid size-11 -translate-y-1/2 place-items-center rounded-full bg-cream/15 text-cream backdrop-blur transition-colors hover:bg-cream/25 sm:right-6 sm:size-12"
                >
                  <ChevronRight className="size-6" />
                </button>

                <div className="absolute inset-x-0 bottom-4 z-20 flex flex-wrap justify-center gap-2 px-4">
                  {images.map((image, index) => (
                    <button
                      key={image.id}
                      type="button"
                      onClick={() => goTo(index)}
                      aria-label={`Фото ${index + 1}`}
                      aria-current={index === safeIndex ? "true" : undefined}
                      className={cn(
                        "h-2 rounded-full transition-all",
                        index === safeIndex
                          ? "w-7 bg-cream"
                          : "w-2 bg-cream/40 hover:bg-cream/70",
                      )}
                    />
                  ))}
                </div>
              </>
            )}
          </DialogContent>
        </Dialog>
      )}

      {/* {similar.length > 0 && (
        <section className="mt-16">
          <div className="mb-6 flex items-end justify-between">
            <h2 className="font-display text-2xl font-bold">Ещё рядом</h2>
            <Link
              href="/"
              className="inline-flex items-center gap-1 text-sm font-semibold text-ink-400 hover:text-ink"
            >
              Все площадки <ArrowUpRight className="size-4" />
            </Link>
          </div>
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {similar.map((v) => (
              <VenueCard key={v.id} venue={v} compact />
            ))}
          </div>
        </section>
      )} */}
    </div>
  );
}
