"use client";

import Image from "next/image";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { Check } from "lucide-react";
import { Logo } from "../ui/logo";
import { cn } from "@/shared/lib/utils";

const tabs = [
  { href: "/auth/login", label: "Вход" },
  { href: "/auth/register", label: "Регистрация" },
];

const benefits = [
  "Личная коллекция площадок",
  "Отзывы и оценки сообщества",
  "Новые места рядом с домом",
];

export function AuthFrame({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();

  return (
    <div className="mx-auto max-w-7xl px-5 py-8 sm:px-8 sm:py-12">
      <div className="overflow-hidden rounded-[2.5rem] border border-ink-200/70 bg-white shadow-soft lg:grid lg:min-h-[730px] lg:grid-cols-[1.02fr_1fr]">
        <aside className="relative hidden overflow-hidden bg-ink p-10 text-cream lg:flex lg:flex-col lg:justify-between xl:p-12">
          <div className="pointer-events-none absolute -right-24 -top-24 size-80 rounded-full bg-lime/20 blur-[100px]" />
          <div className="pointer-events-none absolute -bottom-20 -left-20 size-72 rounded-full bg-lime/10 blur-[90px]" />
          <div className="grain pointer-events-none absolute inset-0 opacity-30" />

          <div className="relative">
            <Logo variant="light" />

            <p className="mt-12 inline-flex items-center gap-2 text-xs font-semibold uppercase tracking-[0.2em] text-lime">
              <span className="size-1.5 rounded-full bg-lime" />
              Сообщество MakeSport
            </p>
            <h1 className="mt-4 font-display text-[2.1rem] font-extrabold leading-[1.08] tracking-tight xl:text-[2.6rem]">
              Спорт начинается
              <br />с одного <span className="text-lime">шага</span>
            </h1>
            <p className="mt-4 max-w-sm text-sm leading-relaxed text-cream/60">
              Сохраняйте любимые площадки, делитесь находками и тренируйтесь
              там, где действительно удобно.
            </p>

            <ul className="mt-8 space-y-3">
              {benefits.map((item) => (
                <li
                  key={item}
                  className="flex items-center gap-3 text-sm text-cream/80"
                >
                  <span className="grid size-6 shrink-0 place-items-center rounded-full bg-lime/15 text-lime">
                    <Check className="size-3.5" />
                  </span>
                  {item}
                </li>
              ))}
            </ul>
          </div>

          <div className="relative mt-10">
            <div className="relative h-52 overflow-hidden rounded-3xl border border-cream/10 bg-ink-800">
              <div
                className="absolute inset-0 opacity-50"
                style={{
                  backgroundImage:
                    "linear-gradient(rgba(246,247,241,0.06) 1px, transparent 1px), linear-gradient(90deg, rgba(246,247,241,0.06) 1px, transparent 1px)",
                  backgroundSize: "34px 34px",
                }}
              />
              <span className="absolute left-1/2 top-1/2 size-64 -translate-x-1/2 -translate-y-1/2 rounded-full border border-lime/15" />
              <span className="absolute left-1/2 top-1/2 size-40 -translate-x-1/2 -translate-y-1/2 rounded-full border border-lime/20" />
              <span className="absolute left-1/2 top-1/2 size-16 -translate-x-1/2 -translate-y-1/2 rounded-full bg-lime/10 blur-md" />

              <span
                className="marker-pin absolute"
                style={{ top: "16%", left: "15%" }}
              >
                <span>⚽</span>
              </span>
              <span
                className="marker-pin absolute"
                style={{ top: "24%", left: "55%" }}
              >
                <span>🏀</span>
              </span>
              <span
                className="marker-pin absolute"
                style={{ top: "18%", left: "78%" }}
              >
                <span>🎾</span>
              </span>
              <span
                className="marker-pin marker-pin--active absolute"
                style={{ top: "36%", left: "30%" }}
              >
                <span>🏋️</span>
              </span>

              <span className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 rounded-full bg-lime px-3 py-1 text-[10px] font-bold uppercase tracking-wider text-ink shadow-lime">
                вы здесь
              </span>

              <div className="absolute inset-x-4 bottom-4 flex items-center justify-between rounded-2xl border border-cream/10 bg-ink/80 px-4 py-3 backdrop-blur">
                <div>
                  <p className="text-[10px] font-semibold uppercase tracking-wider text-cream/40">
                    В радиусе 2 км
                  </p>
                  <p className="mt-1 font-display text-sm font-semibold">
                    18 площадок · 6 бесплатных
                  </p>
                </div>
                <span className="grid size-8 place-items-center rounded-full bg-lime text-xs font-bold text-ink">
                  →
                </span>
              </div>
            </div>

            {/* <div className="mt-5 flex items-center gap-3">
              <div className="flex -space-x-2.5">
                {[
                  "photo-1534528741775-53994a69daeb",
                  "photo-1500648767791-00dcc994a43e",
                  "photo-1531123897727-8f129e1688ce",
                ].map((id) => (
                  <span
                    key={id}
                    className="relative size-9 overflow-hidden rounded-full border-2 border-ink"
                  >
                    <Image
                      src={`https://images.unsplash.com/${id}?auto=format&fit=crop&w=80&h=80&q=70`}
                      alt=""
                      fill
                      sizes="36px"
                      className="object-cover"
                    />
                  </span>
                ))}
              </div>
              <p className="text-xs text-cream/50">
                <span className="font-semibold text-cream">
                  2 400+ участников
                </span>{" "}
                уже собирают свои маршруты
              </p>
            </div> */}
          </div>
        </aside>

        <div className="flex min-h-[640px] flex-col bg-white">
          <div className="flex items-center gap-3 border-b border-ink-100 px-5 py-4 sm:px-8 lg:px-10">
            <nav className="mx-auto flex w-full max-w-xs rounded-full border border-ink-200/80 bg-cream/70 p-1.5 lg:ml-auto lg:mr-0 lg:w-auto">
              {tabs.map((tab) => (
                <Link
                  key={tab.href}
                  href={tab.href}
                  className={cn(
                    "flex-1 rounded-full px-4 py-2 text-center text-[13px] font-semibold transition-colors lg:flex-none",
                    pathname === tab.href
                      ? "bg-ink text-cream"
                      : "text-ink-500 hover:text-ink",
                  )}
                >
                  {tab.label}
                </Link>
              ))}
            </nav>
          </div>

          <div className="flex flex-1 flex-col justify-center px-5 py-8 sm:px-8 lg:px-10 lg:py-10">
            {children}
          </div>
        </div>
      </div>
    </div>
  );
}
