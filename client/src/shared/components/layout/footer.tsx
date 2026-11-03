import Link from "next/link";
import { ArrowUpRight, Send } from "lucide-react";
import { Logo } from "../ui/logo";

const columns = [
  {
    title: "Платформа",
    links: [
      { label: "Все площадки", href: "/" },
      { label: "Карта", href: "/map" },
      { label: "Добавить площадку", href: "/add" },
      { label: "Мой профиль", href: "/profile" },
    ],
  },
  {
    title: "Сообщество",
    links: [
      { label: "Как это работает", href: "/#how" },
      { label: "Правила модерации", href: "/#faq" },
      { label: "Стать волонтёром", href: "/#community" },
    ],
  },
];

export function Footer() {
  return (
    <footer className="relative mt-24 overflow-hidden bg-ink text-cream">
      <div className="pointer-events-none absolute -right-24 -top-24 size-80 rounded-full bg-lime/20 blur-3xl" />
      <div className="relative mx-auto max-w-7xl px-5 py-16 sm:px-8">
        <div className="grid gap-12 lg:grid-cols-[1.4fr_1fr_1fr_1.2fr]">
          <div>
            <Logo variant="light" />
            <p className="mt-5 max-w-xs text-sm leading-relaxed text-cream/60">
              Краудсорсинговая карта спорта. Находите площадки рядом, добавляйте
              новые и делайте свой город активнее.
            </p>
            <div className="mt-6 flex gap-2">
              {[Send /*Instagram, Youtube*/].map((Icon, i) => (
                <a
                  key={i}
                  href="#"
                  className="grid size-10 place-items-center rounded-full border border-cream/15 text-cream/70 transition-colors hover:border-lime hover:text-lime"
                >
                  <Icon className="size-4" />
                </a>
              ))}
            </div>
          </div>

          {columns.map((col) => (
            <div key={col.title}>
              <h4 className="text-xs font-semibold uppercase tracking-[0.18em] text-cream/40">
                {col.title}
              </h4>
              <ul className="mt-4 space-y-3">
                {col.links.map((link) => (
                  <li key={link.label}>
                    <Link
                      href={link.href}
                      className="text-sm text-cream/75 transition-colors hover:text-lime"
                    >
                      {link.label}
                    </Link>
                  </li>
                ))}
              </ul>
            </div>
          ))}

          <div>
            <h4 className="text-xs font-semibold uppercase tracking-[0.18em] text-cream/40">
              Есть площадка?
            </h4>
            <p className="mt-4 text-sm text-cream/60">
              Добавьте её на карту — это займёт меньше двух минут.
            </p>
            <Link
              href="/add"
              className="mt-5 inline-flex items-center gap-2 rounded-full bg-lime px-5 py-3 text-sm font-semibold text-ink transition-transform hover:-translate-y-0.5"
            >
              Предложить площадку
              <ArrowUpRight className="size-4" />
            </Link>
          </div>
        </div>

        <div className="mt-14 flex flex-col gap-4 border-t border-cream/10 pt-6 text-xs text-cream/40 sm:flex-row sm:items-center sm:justify-between">
          <p>© {new Date().getFullYear()} MakeSport. Сделано сообществом.</p>
          <div className="flex gap-6">
            <a href="#" className="transition-colors hover:text-cream">
              Политика конфиденциальности
            </a>
            <a href="#" className="transition-colors hover:text-cream">
              Условия использования
            </a>
          </div>
        </div>
      </div>
    </footer>
  );
}
