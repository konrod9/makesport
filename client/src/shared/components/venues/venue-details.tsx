"use client";

import Image from "next/image";
import Link from "next/link";
import { notFound } from "next/navigation";
import {
  ArrowLeft,
  CalendarClock,
  Check,
  Clock,
  MapPin,
  Share2,
  Star,
  Sun,
  Wallet,
} from "lucide-react";
import { Badge } from "@/shared/components/ui/badge";
import { useGetVenueById } from "@/features/venues/model/use-get-venue-by-id";
import { formatDistance } from "@/shared/lib/format";

export function VenueDetails({ id }: { id: string }) {
  const { data, isPending, isError } = useGetVenueById(id);

  if (isPending) return null; //<VenuePageSkeleton />;
  if (isError || !data?.result) notFound();

  const venue = data.result;

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
        <div className="relative aspect-[16/9] max-h-[520px] w-full sm:aspect-[21/9]">
          <Image
            src={venue.images[0].url}
            alt={venue.title}
            fill
            unoptimized
            priority
            sizes="100vw"
            className="object-cover"
          />
          <div className="absolute inset-0 bg-gradient-to-t from-ink/80 via-ink/10 to-transparent" />

          <div className="absolute left-5 top-5 flex flex-wrap gap-2">
            <Badge tone="lime">{venue.isFree ? "Бесплатно" : "Платно"}</Badge>
            <Badge tone="dark">
              <span className="size-1.5 rounded-full bg-lime" />
              {venue.isOpen ? "Сейчас открыто" : "Закрыто"}
            </Badge>
          </div>

          <button className="absolute right-5 top-5 grid size-11 place-items-center rounded-full bg-cream/90 text-ink backdrop-blur transition-colors hover:bg-cream">
            <Share2 className="size-4" />
          </button>

          <div className="absolute inset-x-5 bottom-5 sm:inset-x-8 sm:bottom-8">
            <div className="flex flex-wrap gap-2">
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
            <h1 className="mt-3 font-display text-3xl font-bold leading-tight text-cream sm:text-4xl lg:text-5xl">
              {venue.title}
            </h1>
            <p className="mt-3 flex flex-wrap items-center gap-x-5 gap-y-2 text-sm text-cream/75">
              <span className="inline-flex items-center gap-1.5">
                <MapPin className="size-4" /> {venue.address.city},{" "}
                {venue.address.street}
              </span>
              <span className="inline-flex items-center gap-1.5">
                <Star className="size-4 fill-lime text-lime" /> {venue.rating} ·{" "}
                {venue.reviewCount} отзывов
              </span>
              {venue.distanceMeters && (
                <span>{formatDistance(venue.distanceMeters)} от вас</span>
              )}
            </p>
          </div>
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
