"use client";

import Image from "next/image";
import Link from "next/link";
import { ReactNode, useEffect, useState } from "react";
import {
  Award,
  Camera,
  ChevronRight,
  Heart,
  LayoutGrid,
  LogOut,
  Mail,
  MapPin,
  MessageSquare,
  Pencil,
  Plus,
  Settings,
  Star,
  Trophy,
  X,
} from "lucide-react";
import { Badge } from "../ui/badge";
import { cn } from "@/shared/lib/utils";
import { useAuthSession } from "@/features/auth/model/auth-store";
import { useLogout } from "@/features/auth/model/use-logout";
import { RoleBadge } from "./role-badge";

//const initialInterests: Sport[] = ["Бег", "Баскетбол", "Воркаут"];

const tabs = [
  { id: "overview", label: "Обзор", icon: LayoutGrid },
  { id: "venues", label: "Мои площадки", icon: MapPin, count: 0 },
  { id: "reviews", label: "Отзывы", icon: MessageSquare, count: 0 },
  { id: "achievements", label: "Достижения", icon: Award },
] as const;

type TabId = (typeof tabs)[number]["id"];

// const myVenues = ["gorky-park", "krylatskoe", "sokolniki"]
//   .map((id) => venues.find((v) => v.id === id))
//   .filter((v): v is Venue => Boolean(v));

const stats = [
  { icon: MapPin, value: "0", label: "площадки" },
  { icon: Star, value: "0", label: "отзывов" },
  { icon: Trophy, value: "0", label: "баллов" },
  { icon: Heart, value: "0", label: "избранных" },
];

type Achievement = {
  icon: ReactNode;
  title: string;
  text: string;
  unlocked: boolean;
};

const achievements: Achievement[] = [];

export function ProfileView() {
  const [tab, setTab] = useState<TabId>("overview");
  // const [interests, setInterests] = useState<Sport[]>(initialInterests);
  const [editing, setEditing] = useState(false);
  // const [draftInterests, setDraftInterests] =
  //   useState<Sport[]>(initialInterests);
  const [helpful, setHelpful] = useState<string[]>([]);
  const [notifications, setNotifications] = useState({
    nearby: true,
    replies: true,
    digest: false,
  });

  const { isAuthenticated, isHydrated, user: sessionUser } = useAuthSession();
  const { logout, isPending: isLoggingOut } = useLogout();

  const contactRows = [
    { id: "email", icon: Mail, value: sessionUser?.email },
  ].filter((row) => Boolean(row.value));

  useEffect(() => {
    if (!editing) return;

    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";

    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape") setEditing(false);
    };
    window.addEventListener("keydown", onKeyDown);

    return () => {
      document.body.style.overflow = previousOverflow;
      window.removeEventListener("keydown", onKeyDown);
    };
  }, [editing]);

  function openEdit() {
    // setDraftInterests(interests);
    setEditing(true);
  }

  // function toggleDraftSport(sport: Sport) {
  //   setDraftInterests((prev) =>
  //     prev.includes(sport) ? prev.filter((s) => s !== sport) : [...prev, sport],
  //   );
  // }

  function saveProfile(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    // setInterests(draftInterests);
    setEditing(false);
  }

  return (
    <div className="mx-auto max-w-7xl px-5 py-8 sm:px-8 sm:py-12">
      <div className="overflow-hidden rounded-[2.5rem] border border-ink-200/70 bg-white shadow-card">
        <div className="relative h-44 sm:h-56 lg:h-64">
          <Image
            src="https://images.unsplash.com/photo-1461896836934-ffe607ba8211?auto=format&fit=crop&w=1600&q=80"
            alt=""
            fill
            priority
            sizes="100vw"
            className="object-cover"
          />
          <div className="absolute inset-0 bg-gradient-to-t from-ink/90 via-ink/40 to-ink/10" />
          <div className="grain pointer-events-none absolute inset-0 opacity-30" />
          <div className="pointer-events-none absolute -right-16 -top-20 size-64 rounded-full bg-lime/30 blur-[90px]" />

          <div className="absolute inset-x-5 top-5 flex items-start justify-between gap-3 sm:inset-x-7 sm:top-6">
            <RoleBadge
              role={sessionUser?.role ?? "User"}
              tone="dark"
              className="whitespace-nowrap border border-cream/15 bg-ink/70 backdrop-blur"
            ></RoleBadge>
            {/* <Badge
              tone="dark"
              className="whitespace-nowrap border border-cream/15 bg-ink/70 backdrop-blur"
            >
              <span className="size-1.5 rounded-full bg-lime" />
              {sessionUser?.role}
            </Badge> */}
            <div className="flex items-center gap-2">
              <a
                href="#settings"
                aria-label="Настройки"
                className="grid size-10 place-items-center rounded-full border border-cream/20 bg-ink/40 text-cream backdrop-blur transition-colors hover:bg-ink/70"
              >
                <Settings className="size-4" />
              </a>
              <button
                onClick={openEdit}
                className="inline-flex items-center gap-2 rounded-full bg-cream px-4 py-2.5 text-xs font-semibold text-ink transition-all hover:shadow-soft"
              >
                <Pencil className="size-3.5" />
                Редактировать
              </button>
            </div>
          </div>
        </div>

        <div className="px-5 pb-6 sm:px-8 sm:pb-7">
          <div className="flex flex-col gap-4 sm:flex-row sm:items-end">
            <div className="relative -mt-12 w-24 shrink-0 self-start sm:-mt-16 sm:w-28">
              <div className="relative grid size-24 place-items-center overflow-hidden rounded-[1.75rem] border-4 border-white bg-cream shadow-card sm:size-28">
                <div className="flex flex-col items-center gap-1 px-2 text-center text-ink-300">
                  <Camera className="size-6" />
                  <span className="text-[10px] font-semibold leading-tight">
                    Скоро
                  </span>
                </div>
              </div>
              <button
                aria-label="Сменить фото"
                className="absolute -bottom-1 -right-1 grid size-8 place-items-center rounded-full border-4 border-white bg-lime text-ink transition-transform hover:scale-105"
              >
                <Camera className="size-3.5" />
              </button>
            </div>

            <div className="min-w-0 flex-1 sm:pb-1">
              <div className="flex flex-wrap items-center gap-2">
                <h1 className="font-display text-2xl font-extrabold tracking-tight text-ink sm:text-3xl pt-2">
                  {sessionUser?.firstName} {sessionUser?.lastName}
                </h1>
                {/* <span className="inline-flex items-center gap-1 rounded-full bg-lime px-2 py-0.5 text-[10px] font-bold text-ink">
                  <BadgeCheck className="size-3" />
                  Проверен
                </span> */}
              </div>
              <p className="mt-1.5 flex flex-wrap items-center gap-x-3 gap-y-1 text-sm text-ink-400">
                {/* <span className="inline-flex items-center gap-1">
                  <MapPin className="size-3.5" /> {profile.city}
                </span>
                <span className="hidden size-1 rounded-full bg-ink-200 sm:inline-block" />
                <span className="inline-flex items-center gap-1">
                  <CalendarDays className="size-3.5" /> с {profile.joined}
                </span>
                <span className="hidden size-1 rounded-full bg-ink-200 sm:inline-block" />
                <span>@alex.smirnov</span> */}
                <span>@{sessionUser?.userName}</span>
              </p>
              {/* <div className="mt-3 flex flex-wrap gap-1.5">
                {interests.map((sport) => (
                  <span
                    key={sport}
                    className="rounded-full bg-ink-50 px-2.5 py-1 text-[11px] font-semibold text-ink-500"
                  >
                    {sport}
                  </span>
                ))}
              </div> */}
            </div>

            <div className="flex flex-wrap items-center gap-2 sm:pb-1">
              <Link
                href="/add"
                className="group inline-flex items-center gap-2 rounded-full bg-ink px-5 py-3 text-sm font-semibold text-cream transition-all hover:bg-ink-700 hover:shadow-soft"
              >
                <Plus className="size-4 transition-transform group-hover:rotate-90" />
                Добавить площадку
              </Link>
            </div>
          </div>

          <div className="mt-6 grid grid-cols-2 gap-px overflow-hidden rounded-3xl border border-ink-100 bg-ink-100 sm:grid-cols-4">
            {stats.map((stat) => (
              <div
                key={stat.label}
                className="group bg-cream/60 p-4 transition-colors hover:bg-cream sm:p-5"
              >
                <div className="flex items-center gap-2">
                  <span className="grid size-7 place-items-center rounded-lg bg-white text-ink-500 transition-colors group-hover:text-lime-600">
                    <stat.icon className="size-3.5" />
                  </span>
                  <p className="font-display text-xl font-extrabold tracking-tight text-ink sm:text-2xl">
                    {stat.value}
                  </p>
                </div>
                <p className="mt-1.5 text-[11px] font-semibold text-ink-400">
                  {stat.label}
                </p>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className="no-scrollbar mt-6 flex gap-1 overflow-x-auto rounded-full border border-ink-200/70 bg-white p-1.5 shadow-card">
        {tabs.map((item) => {
          const active = tab === item.id;
          return (
            <button
              key={item.id}
              onClick={() => setTab(item.id)}
              className={cn(
                "inline-flex shrink-0 items-center gap-2 rounded-full px-4 py-2.5 text-[13px] font-semibold transition-colors",
                active ? "bg-ink text-cream" : "text-ink-500 hover:text-ink",
              )}
            >
              <item.icon className="size-4" />
              {item.label}
              {"count" in item && (
                <span
                  className={cn(
                    "rounded-full px-1.5 py-0.5 text-[10px] font-bold leading-none",
                    active ? "bg-lime text-ink" : "bg-ink-50 text-ink-400",
                  )}
                >
                  {item.count}
                </span>
              )}
            </button>
          );
        })}
      </div>

      <div className="mt-6 grid grid-cols-1 gap-6 lg:grid-cols-[minmax(0,1fr)_320px] lg:items-start">
        <div key={tab} className="min-w-0 animate-fade-up">
          {tab === "overview" && (
            <div className="space-y-5">
              <section className="rounded-4xl border border-ink-200/70 bg-white p-6 sm:p-7">
                <SectionHead
                  title="О себе"
                  action={
                    <button
                      onClick={openEdit}
                      className="inline-flex items-center gap-1.5 text-xs font-semibold text-ink-400 transition-colors hover:text-ink"
                    >
                      <Pencil className="size-3.5" /> Изменить
                    </button>
                  }
                />
                <p className="mt-3 text-sm leading-relaxed text-ink-500">
                  Здесь скоро будет информация о вас
                </p>

                {/* <div className="mt-5 flex flex-wrap gap-2">
                  {interests.map((sport) => (
                    <span
                      key={sport}
                      className="inline-flex items-center gap-1.5 rounded-full border border-ink-200 bg-cream/60 px-3 py-1.5 text-xs font-semibold text-ink-600"
                    >
                      <span className="size-1.5 rounded-full bg-lime-500" />
                      {sport}
                    </span>
                  ))}
                </div>

                <div className="mt-5 flex flex-wrap gap-x-5 gap-y-2 border-t border-ink-100 pt-5 text-[11px] font-semibold text-ink-400">
                  <span className="inline-flex items-center gap-1.5">
                    <MapPin className="size-3.5" /> {profile.city}
                  </span>
                  <span className="inline-flex items-center gap-1.5">
                    <CalendarDays className="size-3.5" /> с {profile.joined}
                  </span>
                  <span className="inline-flex items-center gap-1.5">
                    <ShieldCheck className="size-3.5" /> E-mail подтверждён
                  </span>
                </div> */}
              </section>

              {/* <section className="rounded-4xl border border-ink-200/70 bg-white p-6 sm:p-7">
                <SectionHead
                  title="Активность"
                  action={
                    <button
                      onClick={() => setTab("reviews")}
                      className="inline-flex items-center gap-1 text-xs font-semibold text-ink-400 transition-colors hover:text-ink"
                    >
                      Вся история <ChevronRight className="size-3.5" />
                    </button>
                  }
                />
                <div className="mt-5">
                  {activity.map((item, index) => (
                    <div key={item.title} className="flex gap-4">
                      <div className="flex flex-col items-center">
                        <span className="grid size-9 shrink-0 place-items-center rounded-xl bg-lime/20 text-lime-600">
                          <item.icon className="size-4" />
                        </span>
                        {index < activity.length - 1 && (
                          <span className="mt-2 w-px flex-1 bg-ink-100" />
                        )}
                      </div>
                      <div
                        className={cn(index < activity.length - 1 && "pb-5")}
                      >
                        <p className="text-sm font-semibold leading-snug text-ink">
                          {item.title}
                        </p>
                        <p className="mt-1 text-[11px] font-medium text-ink-400">
                          {item.time}
                        </p>
                      </div>
                    </div>
                  ))}
                </div>
              </section> */}

              <section className="rounded-4xl border border-ink-200/70 bg-white p-6 sm:p-7">
                <SectionHead
                  title="Мои площадки"
                  action={
                    <button
                      onClick={() => setTab("venues")}
                      className="inline-flex items-center gap-1 text-xs font-semibold text-ink-400 transition-colors hover:text-ink"
                    >
                      {/* Все {myVenues.length}{" "} */}
                      <ChevronRight className="size-3.5" />
                    </button>
                  }
                />
                <p className="mt-3 text-sm leading-relaxed text-ink-500">
                  Здесь скоро будут ваши добавленные площадки
                </p>
                {/* <div className="mt-4 space-y-2.5">
                  {myVenues.map((venue, index) => (
                    <Link
                      key={venue.id}
                      href={`/venue/${venue.id}`}
                      className="group flex items-center gap-3 rounded-2xl border border-ink-100 bg-cream/40 p-2.5 transition-colors hover:bg-cream"
                    >
                      <div className="relative size-14 shrink-0 overflow-hidden rounded-xl">
                        <Image
                          src={venue.image}
                          alt={venue.name}
                          fill
                          sizes="56px"
                          className="object-cover"
                        />
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="truncate text-sm font-semibold text-ink transition-colors group-hover:text-lime-600">
                          {venue.name}
                        </p>
                        <p className="mt-0.5 flex items-center gap-1.5 text-[11px] text-ink-400">
                          <MapPin className="size-3" /> {venue.city}
                          <span className="size-1 rounded-full bg-ink-200" />
                          <Star className="size-3 fill-lime-500 text-lime-500" />
                          {venue.rating}
                        </p>
                      </div>
                      <span
                        className={cn(
                          "hidden shrink-0 rounded-full px-2.5 py-1 text-[10px] font-semibold sm:inline-flex",
                          index === 2
                            ? "bg-clay text-ink-500"
                            : "bg-lime/20 text-ink-600",
                        )}
                      >
                        {index === 2 ? "На модерации" : "Опубликовано"}
                      </span>
                      <ArrowUpRight className="size-4 shrink-0 text-ink-200 transition-colors group-hover:text-ink" />
                    </Link>
                  ))}
                </div> */}
              </section>
            </div>
          )}

          {tab === "venues" && (
            <div className="space-y-5">
              <div className="flex flex-wrap items-end justify-between gap-3">
                <div>
                  <h2 className="font-display text-xl font-bold text-ink">
                    Мои площадки
                  </h2>
                  <p className="mt-1 text-sm text-ink-400">
                    Места, которые вы добавили на карту сообщества
                  </p>
                </div>
                <Link
                  href="/add"
                  className="inline-flex items-center gap-2 rounded-full bg-lime px-5 py-2.5 text-xs font-bold text-ink transition-all hover:shadow-lime"
                >
                  <Plus className="size-3.5" /> Добавить
                </Link>
              </div>

              {/* <div className="grid gap-5 sm:grid-cols-2">
                {myVenues.map((venue) => (
                  <VenueCard key={venue.id} venue={venue} compact />
                ))}
              </div> */}

              <Link
                href="/add"
                className="group flex items-center justify-center gap-2 rounded-4xl border-2 border-dashed border-ink-200 bg-white/50 py-8 text-sm font-semibold text-ink-400 transition-colors hover:border-ink hover:text-ink"
              >
                <span className="grid size-8 place-items-center rounded-full bg-ink-50 transition-colors group-hover:bg-lime group-hover:text-ink">
                  <Plus className="size-4" />
                </span>
                Добавить новую точку на карту
              </Link>
            </div>
          )}

          {tab === "reviews" && (
            <div className="space-y-5">
              <div>
                <h2 className="font-display text-xl font-bold text-ink">
                  Мои отзывы
                </h2>
                <p className="mt-1 text-sm text-ink-400">
                  Помогаете другим выбирать площадку
                </p>
              </div>

              {/* {reviews.map((review) => {
                const voted = helpful.includes(review.id);
                return (
                  <article
                    key={review.id}
                    className="rounded-4xl border border-ink-200/70 bg-white p-6 sm:p-7"
                  >
                    <div className="flex flex-wrap items-start justify-between gap-3">
                      <div>
                        <Link
                          href={`/venue/${review.venueId}`}
                          className="font-display text-base font-semibold text-ink transition-colors hover:text-lime-600"
                        >
                          {review.venueName}
                        </Link>
                        <p className="mt-1 text-[11px] font-medium text-ink-400">
                          {review.date} · отзыв о площадке
                        </p>
                      </div>
                      <span className="flex items-center gap-0.5">
                        {Array.from({ length: 5 }).map((_, i) => (
                          <Star
                            key={i}
                            className={cn(
                              "size-3.5",
                              i < review.rating
                                ? "fill-lime-500 text-lime-500"
                                : "text-ink-200",
                            )}
                          />
                        ))}
                      </span>
                    </div>

                    <p className="mt-4 text-sm leading-relaxed text-ink-500">
                      {review.text}
                    </p>

                    <div className="mt-5 flex items-center justify-between border-t border-ink-100 pt-4">
                      <button
                        onClick={() =>
                          setHelpful((prev) =>
                            prev.includes(review.id)
                              ? prev.filter((id) => id !== review.id)
                              : [...prev, review.id],
                          )
                        }
                        className={cn(
                          "inline-flex items-center gap-1.5 rounded-full border px-3 py-1.5 text-[11px] font-semibold transition-colors",
                          voted
                            ? "border-lime-500 bg-lime/15 text-ink"
                            : "border-ink-200 text-ink-400 hover:text-ink",
                        )}
                      >
                        <ThumbsUp
                          className={cn("size-3.5", voted && "text-lime-600")}
                        />
                        Полезно · {review.helpful + (voted ? 1 : 0)}
                      </button>
                      <button className="text-[11px] font-semibold text-ink-400 transition-colors hover:text-ink">
                        Редактировать
                      </button>
                    </div>
                  </article>
                );
              })} */}
            </div>
          )}

          {tab === "achievements" && (
            <div className="space-y-5">
              <div>
                <h2 className="font-display text-xl font-bold text-ink">
                  Достижения
                </h2>
                <p className="mt-1 text-sm text-ink-400">
                  Награды за вклад в карту спорта
                </p>
              </div>

              {/* <div className="grid gap-4 sm:grid-cols-2">
                {achievements.map((item) => (
                  <div
                    key={item.title}
                    className={cn(
                      "relative overflow-hidden rounded-3xl border p-5 transition-all",
                      item.unlocked
                        ? "border-ink-200/70 bg-white hover:-translate-y-0.5 hover:shadow-card"
                        : "border-dashed border-ink-200 bg-cream/40",
                    )}
                  >
                    <div className="flex items-start justify-between gap-3">
                      <span
                        className={cn(
                          "grid size-11 place-items-center rounded-2xl",
                          item.unlocked
                            ? "bg-lime text-ink"
                            : "bg-ink-50 text-ink-300",
                        )}
                      >
                        <item.icon className="size-5" />
                      </span>
                      <span
                        className={cn(
                          "rounded-full px-2 py-0.5 text-[10px] font-semibold",
                          item.unlocked
                            ? "bg-lime/20 text-ink-600"
                            : "bg-ink-50 text-ink-400",
                        )}
                      >
                        {item.unlocked ? "Получено" : "В процессе"}
                      </span>
                    </div>
                    <h3
                      className={cn(
                        "mt-4 font-display text-base font-semibold",
                        item.unlocked ? "text-ink" : "text-ink-500",
                      )}
                    >
                      {item.title}
                    </h3>
                    <p className="mt-1 text-xs leading-relaxed text-ink-400">
                      {item.text}
                    </p>

                    {!item.unlocked && item.progress !== undefined && (
                      <div className="mt-4">
                        <div className="h-1.5 overflow-hidden rounded-full bg-ink-100">
                          <div
                            className="h-full rounded-full bg-lime-500"
                            style={{
                              width: `${Math.round(
                                (item.progress / (item.goal ?? 1)) * 100,
                              )}%`,
                            }}
                          />
                        </div>
                        <p className="mt-1.5 text-[10px] font-semibold text-ink-400">
                          {item.progress} из {item.goal}
                        </p>
                      </div>
                    )}
                  </div>
                ))}
              </div> */}
            </div>
          )}
        </div>

        <aside className="min-w-0 space-y-4 lg:sticky lg:top-24">
          {/* <div className="relative overflow-hidden rounded-4xl bg-ink p-6 text-cream">
            <div className="pointer-events-none absolute -right-10 -top-10 size-40 rounded-full bg-lime/25 blur-[50px]" />
            <div className="relative">
              <div className="flex items-center justify-between">
                <p className="text-[10px] font-semibold uppercase tracking-[0.18em] text-cream/50">
                  Ваш уровень
                </p>
                <Trophy className="size-5 text-lime" />
              </div>
              <p className="mt-3 font-display text-3xl font-extrabold tracking-tight">
                03<span className="text-lime">/05</span>
              </p>
              <p className="mt-1 text-sm text-cream/60">Активный участник</p>

              <div className="mt-5 h-2 overflow-hidden rounded-full bg-cream/10">
                <div className="h-full w-[72%] rounded-full bg-lime" />
              </div>
              <div className="mt-2 flex items-center justify-between text-[10px] font-semibold text-cream/50">
                <span>720 / 1000 баллов</span>
                <span>до уровня 4</span>
              </div>

              <div className="mt-5 flex items-center gap-2 rounded-2xl border border-cream/10 bg-cream/5 px-3 py-2.5 text-[11px] text-cream/70">
                <Sparkles className="size-3.5 shrink-0 text-lime" />
                +40 баллов за новый отзыв
              </div>
            </div>
          </div> */}

          <div
            id="settings"
            className="rounded-4xl border border-ink-200/70 bg-white p-6"
          >
            <h3 className="text-xs font-semibold uppercase tracking-[0.18em] text-ink-400">
              Контактные данные
            </h3>
            <div className="mt-4 space-y-3">
              {contactRows.map((row) => (
                <div key={row.id} className="flex items-center gap-3">
                  <span className="grid size-9 shrink-0 place-items-center rounded-xl bg-cream text-ink-500">
                    <row.icon className="size-4" />
                  </span>
                  <p className="truncate text-[13px] font-medium text-ink-600">
                    {row.value}
                  </p>
                </div>
              ))}
            </div>
            <button
              onClick={openEdit}
              className="mt-4 flex w-full items-center justify-between rounded-2xl border border-ink-100 bg-cream/50 px-4 py-3 text-xs font-semibold text-ink-500 transition-colors hover:border-ink-200 hover:text-ink"
            >
              Изменить данные
              <ChevronRight className="size-4" />
            </button>
          </div>

          {/* <div className="rounded-4xl border border-ink-200/70 bg-white p-6">
            <h3 className="text-xs font-semibold uppercase tracking-[0.18em] text-ink-400">
              Уведомления
            </h3>
            <div className="mt-2 divide-y divide-ink-100">
              <SwitchRow
                label="Новые места рядом"
                hint="Когда рядом появляется площадка"
                checked={notifications.nearby}
                onChange={(value) =>
                  setNotifications((prev) => ({ ...prev, nearby: value }))
                }
              />
              <SwitchRow
                label="Ответы на отзывы"
                hint="Комментарии к вашим отзывам"
                checked={notifications.replies}
                onChange={(value) =>
                  setNotifications((prev) => ({ ...prev, replies: value }))
                }
              />
              <SwitchRow
                label="Дайджест недели"
                hint="Лучшие места и события"
                checked={notifications.digest}
                onChange={(value) =>
                  setNotifications((prev) => ({ ...prev, digest: value }))
                }
              />
            </div>
          </div> */}

          <Link
            href="/auth/login"
            onClick={() => logout()}
            className="flex w-full items-center justify-center gap-2 rounded-2xl border border-ink-200 bg-white py-3 text-xs font-semibold text-ink-400 transition-colors hover:border-ink-300 hover:text-ink"
          >
            <LogOut className="size-3.5" />
            {isLoggingOut ? "Выходим..." : "Выйти из аккаунта"}
          </Link>
        </aside>
      </div>

      {editing && (
        <div
          className="fixed inset-0 z-[70] flex items-end justify-center bg-ink/60 backdrop-blur-sm sm:items-center sm:p-6"
          onClick={() => setEditing(false)}
          role="dialog"
          aria-modal="true"
          aria-label="Редактирование профиля"
        >
          <div
            className="max-h-[92vh] w-full max-w-lg animate-fade-up overflow-y-auto rounded-t-[2rem] bg-cream p-6 shadow-soft sm:rounded-[2rem] sm:p-8"
            onClick={(event) => event.stopPropagation()}
          >
            <div className="flex items-start justify-between gap-4">
              <div>
                <p className="inline-flex items-center gap-2 text-[10px] font-semibold uppercase tracking-[0.2em] text-lime-600">
                  <span className="size-1.5 rounded-full bg-lime-500" />
                  Настройки профиля
                </p>
                <h2 className="mt-2 font-display text-2xl font-bold text-ink">
                  Редактировать профиль
                </h2>
              </div>
              <button
                onClick={() => setEditing(false)}
                aria-label="Закрыть"
                className="grid size-10 shrink-0 place-items-center rounded-full border border-ink-200 bg-white text-ink-400 transition-colors hover:text-ink"
              >
                <X className="size-4" />
              </button>
            </div>

            <form onSubmit={saveProfile} className="mt-6 space-y-4">
              {/* <label className="block space-y-2">
                <span className="text-[11px] font-semibold uppercase tracking-wider text-ink-500">
                  Имя
                </span>
                <input
                  value={draft.name}
                  onChange={(e) => setDraft({ ...draft, name: e.target.value })}
                  required
                  minLength={2}
                  className={editInput}
                />
              </label>

              <label className="block space-y-2">
                <span className="text-[11px] font-semibold uppercase tracking-wider text-ink-500">
                  Город
                </span>
                <select
                  value={draft.city}
                  onChange={(e) => setDraft({ ...draft, city: e.target.value })}
                  className={editInput}
                >
                  {["Москва", "Санкт-Петербург", "Казань", "Екатеринбург"].map(
                    (city) => (
                      <option key={city}>{city}</option>
                    ),
                  )}
                </select>
              </label>

              <label className="block space-y-2">
                <span className="text-[11px] font-semibold uppercase tracking-wider text-ink-500">
                  О себе
                </span>
                <textarea
                  value={draft.bio}
                  onChange={(e) => setDraft({ ...draft, bio: e.target.value })}
                  rows={4}
                  className={cn(editInput, "h-auto resize-none py-3")}
                />
              </label> */}

              {/* <div className="space-y-2">
                <span className="text-[11px] font-semibold uppercase tracking-wider text-ink-500">
                  Любимые виды спорта
                </span>
                <div className="flex flex-wrap gap-2">
                  {allSports.map((sport) => {
                    const on = draftInterests.includes(sport);
                    return (
                      <button
                        key={sport}
                        type="button"
                        onClick={() => toggleDraftSport(sport)}
                        className={cn(
                          "inline-flex items-center gap-1.5 rounded-full border px-3 py-1.5 text-[12px] font-semibold transition-all",
                          on
                            ? "border-ink bg-ink text-cream"
                            : "border-ink-200 bg-white text-ink-500 hover:border-ink-300",
                        )}
                      >
                        {on && <Check className="size-3 text-lime" />}
                        {sport}
                      </button>
                    );
                  })}
                </div>
              </div> */}

              <div className="flex gap-2 pt-2">
                <button
                  type="button"
                  onClick={() => setEditing(false)}
                  className="flex-1 rounded-full border border-ink-200 bg-white py-3 text-sm font-semibold text-ink transition-colors hover:border-ink-300"
                >
                  Отмена
                </button>
                <button
                  type="submit"
                  className="flex-1 rounded-full bg-lime py-3 text-sm font-bold text-ink transition-all hover:shadow-lime"
                >
                  Сохранить
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

const editInput =
  "h-12 w-full rounded-2xl border border-ink-200 bg-white px-4 text-sm font-medium text-ink outline-none transition-all placeholder:text-ink-400 focus:border-ink focus:ring-4 focus:ring-lime/30";

function SectionHead({
  title,
  action,
}: {
  title: string;
  action?: React.ReactNode;
}) {
  return (
    <div className="flex items-center justify-between gap-3">
      <h2 className="font-display text-lg font-bold text-ink">{title}</h2>
      {action}
    </div>
  );
}

function SwitchRow({
  label,
  hint,
  checked,
  onChange,
}: {
  label: string;
  hint: string;
  checked: boolean;
  onChange: (value: boolean) => void;
}) {
  return (
    <button
      type="button"
      onClick={() => onChange(!checked)}
      className="flex w-full items-center justify-between gap-3 py-3 text-left"
    >
      <span className="min-w-0">
        <span className="block text-[13px] font-semibold text-ink">
          {label}
        </span>
        <span className="mt-0.5 block text-[11px] leading-snug text-ink-400">
          {hint}
        </span>
      </span>
      <span
        className={cn(
          "relative h-6 w-11 shrink-0 rounded-full transition-colors",
          checked ? "bg-ink" : "bg-ink-200",
        )}
      >
        <span
          className={cn(
            "absolute top-0.5 size-5 rounded-full bg-white shadow-sm transition-all",
            checked ? "left-[22px]" : "left-0.5",
          )}
        />
      </span>
    </button>
  );
}
