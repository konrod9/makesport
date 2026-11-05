import Image from "next/image";
import Link from "next/link";
import {
  ArrowRight,
  ArrowUpRight,
  Compass,
  MapPin,
  Plus,
  Sparkles,
} from "lucide-react";

export default function Home() {
  return (
    <>
      <Hero />
      <Marquee />
      <Stats />
      <HowItWorks />
      <MapTeaser />
      <Community />
    </>
  );
}
function Hero() {
  return (
    <section className="relative overflow-hidden">
      <div className="pointer-events-none absolute -left-40 top-10 size-[30rem] rounded-full bg-lime/30 blur-[120px]" />
      <div className="pointer-events-none absolute -right-32 top-40 size-[26rem] rounded-full bg-clay blur-[100px]" />

      <div className="relative mx-auto grid max-w-7xl gap-12 px-5 pb-16 pt-12 sm:px-8 lg:grid-cols-[1.05fr_0.95fr] lg:items-center lg:gap-8 lg:pb-24 lg:pt-16">
        <div className="animate-fade-up">
          <span className="inline-flex items-center gap-2 rounded-full border border-ink-200 bg-white/70 px-3.5 py-1.5 text-xs font-semibold text-ink-500 backdrop-blur">
            <span className="relative flex size-2">
              <span className="absolute inline-flex size-full animate-ping rounded-full bg-lime-500 opacity-75" />
              <span className="relative inline-flex size-2 rounded-full bg-lime-500" />
            </span>
            {/* Краудсорсинг · {venues.length} площадок на карте */}
            Краудсорсинг · смотри площадки на карте
          </span>

          <h1 className="mt-6 font-display text-[2.6rem] font-extrabold leading-[1.04] tracking-tight text-ink sm:text-6xl lg:text-[4.1rem]">
            Найди свою
            <br />
            <span className="relative inline-block">
              <span className="relative z-10">площадку</span>
              <svg
                viewBox="0 0 300 18"
                className="absolute -bottom-1 left-0 h-4 w-full text-lime"
                preserveAspectRatio="none"
              >
                <path
                  d="M2 12c60-8 130-10 296-6"
                  stroke="currentColor"
                  strokeWidth="8"
                  strokeLinecap="round"
                  fill="none"
                />
              </svg>
            </span>
            <br />
            рядом с домом
          </h1>

          <p className="mt-6 max-w-lg text-lg leading-relaxed text-ink-500">
            Карта спорта, которую наполняют люди. Турники во дворе, корты,
            скейт-парки и стадионы — всё, что нужно, чтобы начать заниматься
            сегодня.
          </p>

          <div className="mt-8 flex flex-col gap-3 sm:flex-row">
            <Link
              href="/map"
              className="group inline-flex items-center justify-center gap-2 rounded-full bg-ink px-7 py-4 text-sm font-semibold text-cream transition-all hover:bg-ink-700 hover:shadow-soft"
            >
              <Compass className="size-4" />
              Найти на карте
              <ArrowRight className="size-4 transition-transform group-hover:translate-x-1" />
            </Link>
            <Link
              href="/add"
              className="group inline-flex items-center justify-center gap-2 rounded-full border border-ink-300 bg-white/70 px-7 py-4 text-sm font-semibold text-ink backdrop-blur transition-all hover:border-ink hover:bg-white"
            >
              <Plus className="size-4" />
              Добавить площадку
            </Link>
          </div>

          {/* <div className="mt-10 flex flex-wrap items-center gap-x-8 gap-y-4">
            <div className="flex -space-x-2.5">
              {[
                "photo-1534528741775-53994a69daeb",
                "photo-1500648767791-00dcc994a43e",
                "photo-1531123897727-8f129e1688ce",
                "photo-1506794778202-cad84cf45f1d",
              ].map((id) => (
                <span
                  key={id}
                  className="relative size-10 overflow-hidden rounded-full border-2 border-cream"
                >
                  <Image
                    src={`https://make-sport.ru/storage/pictures/9279a179-68da-4335-a084-2d85796c7bd7`}
                    alt=""
                    fill
                    sizes="40px"
                    className="object-cover"
                  />
                </span>
              ))}
            </div>
            <div>
              <p className="font-display text-sm font-bold text-ink">
                2 400+ участников
              </p>
              <p className="text-xs text-ink-400">уже добавили свои площадки</p>
            </div>
          </div> */}
        </div>

        <div className="relative animate-fade-up [animation-delay:120ms]">
          <div className="relative aspect-[4/5] overflow-hidden rounded-[2.5rem] border border-ink-200 bg-ink shadow-soft sm:aspect-[5/5] lg:aspect-[4/5]">
            <Image
              src="https://make-sport.ru/storage/pictures/9279a179-68da-4335-a084-2d85796c7bd7"
              alt="Спортивная площадка"
              fill
              priority
              sizes="(max-width: 1024px) 100vw, 45vw"
              className="object-cover"
            />
            <div className="absolute inset-0 bg-gradient-to-t from-ink/85 via-ink/10 to-transparent" />

            <div className="absolute left-5 top-5 flex items-center gap-2 rounded-full bg-cream/95 px-3.5 py-2 text-xs font-semibold text-ink backdrop-blur">
              <MapPin className="size-3.5 text-lime-600" />
              Москва · Хамовники
            </div>

            <div className="absolute inset-x-5 bottom-5 rounded-3xl border border-white/20 bg-white/10 p-4 backdrop-blur-xl">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-[11px] font-semibold uppercase tracking-wider text-cream/60">
                    Ближайшая площадка
                  </p>
                  <p className="mt-1 font-display text-base font-semibold text-cream">
                    Воркаут-зона в Парке Горького
                  </p>
                </div>
                <span className="grid size-10 place-items-center rounded-full bg-lime text-ink">
                  <ArrowUpRight className="size-4" />
                </span>
              </div>
              <div className="mt-3 flex items-center gap-4 text-[11px] font-medium text-cream/70">
                <span>★ 4.6</span>
                <span>·</span>
                <span>350 м</span>
                <span>·</span>
                <span>круглосуточно</span>
              </div>
            </div>
          </div>

          <div className="absolute -left-4 top-1/3 hidden animate-float rounded-3xl border border-ink-200 bg-white p-4 shadow-card sm:block">
            <p className="text-[10px] font-semibold uppercase tracking-wider text-ink-400">
              Бесплатных
            </p>
            <p className="mt-1 font-display text-2xl font-extrabold text-ink">
              68%
            </p>
            <div className="mt-2 h-1.5 w-20 overflow-hidden rounded-full bg-ink-100">
              <div className="h-full w-[68%] rounded-full bg-lime-500" />
            </div>
          </div>

          <div className="absolute -right-3 bottom-24 hidden animate-float rounded-2xl bg-ink px-4 py-3 text-cream shadow-soft [animation-delay:1.5s] sm:block">
            <p className="flex items-center gap-2 text-xs font-semibold">
              <Sparkles className="size-3.5 text-lime" />
              +12 новых за неделю
            </p>
          </div>
        </div>
      </div>
    </section>
  );
}

function Marquee() {
  const items = [
    "Футбол",
    "Баскетбол",
    "Теннис",
    "Воркаут",
    "Волейбол",
    "Бег",
    "Скейтборд",
    "Велоспорт",
    "и другие",
  ];
  return (
    <div className="border-y border-ink-200/70 bg-ink py-4 text-cream">
      <div className="flex overflow-hidden">
        <div className="flex shrink-0 animate-marquee items-center gap-8 pr-8">
          {[...items, ...items, ...items, ...items].map((item, i) => (
            <span
              key={i}
              className="flex items-center gap-8 whitespace-nowrap font-display text-sm font-semibold uppercase tracking-[0.2em] text-cream/80"
            >
              {item}
              <span className="size-1.5 rounded-full bg-lime" />
            </span>
          ))}
        </div>
        <div
          className="flex shrink-0 animate-marquee items-center gap-8 pr-8"
          aria-hidden
        >
          {[...items, ...items, ...items, ...items].map((item, i) => (
            <span
              key={i}
              className="flex items-center gap-8 whitespace-nowrap font-display text-sm font-semibold uppercase tracking-[0.2em] text-cream/80"
            >
              {item}
              <span className="size-1.5 rounded-full bg-lime" />
            </span>
          ))}
        </div>
      </div>
    </div>
  );
}

function Stats() {
  const stats = [
    { value: "1 240", label: "площадок на карте", note: "в 4 городах" },
    { value: "68%", label: "бесплатных локаций", note: "без абонемента" },
    {
      value: "2 400",
      label: "участников сообщества",
      note: "и это только начало",
    },
  ];
  return (
    <section className="mx-auto max-w-7xl px-5 py-16 sm:px-8 sm:py-20">
      <div className="grid gap-px overflow-hidden rounded-4xl border border-ink-200/70 bg-ink-200/70 sm:grid-cols-2 lg:grid-cols-3">
        {stats.map((s) => (
          <div
            key={s.label}
            className="group bg-cream p-6 transition-colors hover:bg-white sm:p-8"
          >
            <p className="font-display text-4xl font-extrabold tracking-tight text-ink transition-colors group-hover:text-lime-600">
              {s.value}
            </p>
            <p className="mt-2 text-sm font-semibold text-ink">{s.label}</p>
            <p className="mt-1 text-xs text-ink-400">{s.note}</p>
          </div>
        ))}
      </div>
    </section>
  );
}

function HowItWorks() {
  const steps = [
    {
      n: "01",
      title: "Откройте карту",
      text: "Определите геолокацию или выберите город — карта покажет площадки вокруг вас.",
    },
    {
      n: "02",
      title: "Выберите площадку",
      text: "Смотрите покрытие, освещение, часы работы, фото и отзывы других спортсменов.",
    },
    {
      n: "03",
      title: "Добавьте свою",
      text: "Знаете хорошее место, которого нет на карте? Отметьте его — и оно появится у всех.",
    },
  ];
  return (
    <section id="how" className="mx-auto max-w-7xl px-5 py-16 sm:px-8 sm:py-24">
      <div className="grid gap-12 lg:grid-cols-[0.9fr_1.1fr] lg:items-center">
        <div>
          <p className="mb-3 inline-flex items-center gap-2 text-xs font-semibold uppercase tracking-[0.2em] text-lime-600">
            <span className="size-1.5 rounded-full bg-lime-500" />
            Как это работает
          </p>
          <h2 className="font-display text-3xl font-bold leading-tight text-ink sm:text-4xl lg:text-[2.75rem]">
            Спорт начинается
            <br />с одного клика
          </h2>
          <p className="mt-5 max-w-md text-ink-500">
            Мы делаем площадки видимыми. Каждая точка на карте добавлена
            человеком, который здесь уже тренировался.
          </p>
          <Link
            href="/add"
            className="group mt-8 inline-flex items-center gap-2 rounded-full bg-lime px-6 py-3.5 text-sm font-bold text-ink transition-all hover:shadow-lime"
          >
            Предложить место
            <ArrowRight className="size-4 transition-transform group-hover:translate-x-1" />
          </Link>
        </div>

        <div className="space-y-3">
          {steps.map((step) => (
            <div
              key={step.n}
              className="group flex gap-5 rounded-4xl border border-ink-200/70 bg-white p-6 transition-all hover:-translate-y-0.5 hover:shadow-card sm:p-7"
            >
              <span className="font-display text-sm font-bold text-lime-600">
                {step.n}
              </span>
              <div>
                <h3 className="font-display text-lg font-semibold text-ink">
                  {step.title}
                </h3>
                <p className="mt-1.5 text-sm leading-relaxed text-ink-500">
                  {step.text}
                </p>
              </div>
              <ArrowUpRight className="ml-auto size-5 shrink-0 self-center text-ink-200 transition-all group-hover:text-ink" />
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}

function MapTeaser() {
  return (
    <section className="mx-auto max-w-7xl px-5 py-8 sm:px-8">
      <div className="relative overflow-hidden rounded-[2.75rem] bg-ink px-6 py-14 text-cream sm:px-12 sm:py-20">
        <div className="pointer-events-none absolute -right-20 -top-20 size-96 rounded-full bg-lime/25 blur-[110px]" />
        <div className="pointer-events-none absolute bottom-0 left-1/3 size-72 rounded-full bg-lime/10 blur-[90px]" />
        <div className="grain pointer-events-none absolute inset-0 opacity-40" />

        <div className="relative grid gap-10 lg:grid-cols-2 lg:items-center">
          <div>
            <p className="mb-3 inline-flex items-center gap-2 text-xs font-semibold uppercase tracking-[0.2em] text-lime">
              <span className="size-1.5 rounded-full bg-lime" />
              Живая карта
            </p>
            <h2 className="font-display text-3xl font-bold leading-tight sm:text-4xl lg:text-[2.9rem]">
              Весь спорт города
              <br />
              на одном экране
            </h2>
            <p className="mt-5 max-w-md text-cream/60">
              Кластеры, фильтры по видам спорта и маршрут до площадки.
              Открывается мгновенно — даже на телефоне во дворе.
            </p>
            <Link
              href="/map"
              className="group mt-8 inline-flex items-center gap-2 rounded-full bg-lime px-6 py-3.5 text-sm font-bold text-ink transition-all hover:shadow-lime"
            >
              <Compass className="size-4" />
              Перейти к карте
              <ArrowRight className="size-4 transition-transform group-hover:translate-x-1" />
            </Link>
          </div>

          <div className="relative">
            <div className="overflow-hidden rounded-3xl border border-cream/10 bg-ink-800">
              <div className="relative aspect-[4/3]">
                <Image
                  src="https://images.unsplash.com/photo-1524661135-423995f22d0b?auto=format&fit=crop&w=1200&q=80"
                  alt="Карта спортивных площадок"
                  fill
                  sizes="(max-width: 1024px) 100vw, 50vw"
                  className="object-cover opacity-60"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-ink via-ink/30 to-transparent" />
                {[
                  { top: "26%", left: "22%", label: "⚽" },
                  { top: "48%", left: "58%", label: "🏀" },
                  { top: "68%", left: "34%", label: "🏋️" },
                  { top: "36%", left: "76%", label: "🎾" },
                ].map((p, i) => (
                  <span
                    key={i}
                    className="absolute grid size-10 -translate-x-1/2 -translate-y-1/2 place-items-center rounded-full border-2 border-lime bg-ink text-base shadow-soft"
                    style={{ top: p.top, left: p.left }}
                  >
                    {p.label}
                  </span>
                ))}
                <div className="absolute inset-x-4 bottom-4 rounded-2xl border border-cream/10 bg-ink/80 p-4 backdrop-blur">
                  <p className="text-xs font-semibold text-cream/50">
                    В радиусе 2 км
                  </p>
                  <p className="mt-1 font-display text-lg font-semibold">
                    18 площадок · 6 бесплатных
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}

function Community() {
  return (
    <section
      id="community"
      className="mx-auto max-w-7xl px-5 py-16 sm:px-8 sm:py-24"
    >
      <div className="grid gap-6 lg:grid-cols-3">
        <div className="rounded-4xl border border-ink-200/70 bg-clay p-8 lg:col-span-2">
          <div className="flex items-start justify-between gap-6">
            <div>
              <p className="mb-3 inline-flex items-center gap-2 text-xs font-semibold uppercase tracking-[0.2em] text-ink-500">
                <span className="size-1.5 rounded-full bg-ink" />
                Сообщество
              </p>
              <h2 className="font-display text-2xl font-bold leading-tight sm:text-3xl">
                Площадку добавил кто-то
                <br />
                из вашего района
              </h2>
            </div>
          </div>
          <div className="mt-8 grid gap-3 sm:grid-cols-2">
            {[
              {
                name: "Аня",
                text: "Нашла корт за домом — играем каждую субботу",
                meta: "Москва",
              },
              {
                name: "Марк",
                text: "Добавил турники, через день пришли соседи",
                meta: "Санкт-Петербург",
              },
            ].map((c) => (
              <div key={c.name} className="rounded-3xl bg-cream/80 p-5">
                <p className="text-sm leading-relaxed text-ink-600">
                  «{c.text}»
                </p>
                <p className="mt-4 text-xs font-semibold text-ink">
                  {c.name}{" "}
                  <span className="font-medium text-ink-400">· {c.meta}</span>
                </p>
              </div>
            ))}
          </div>
        </div>

        <div className="flex flex-col justify-between rounded-4xl bg-lime p-8">
          <div>
            <p className="text-xs font-semibold uppercase tracking-[0.2em] text-ink/50">
              Присоединяйтесь
            </p>
            <h3 className="mt-4 font-display text-2xl font-bold leading-tight text-ink">
              Знаете место,
              <br />
              которого нет?
            </h3>
            <p className="mt-3 text-sm text-ink/70">
              Добавьте его за пару минут. Модерация — в течение суток.
            </p>
          </div>
          <Link
            href="/add"
            className="group mt-8 inline-flex items-center justify-between rounded-full bg-ink px-5 py-3.5 text-sm font-semibold text-cream transition-transform hover:-translate-y-0.5"
          >
            Добавить площадку
            <ArrowUpRight className="size-4 transition-transform group-hover:rotate-45" />
          </Link>
        </div>
      </div>
    </section>
  );
}
