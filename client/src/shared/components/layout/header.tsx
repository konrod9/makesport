import Link from "next/link";
import { usePathname } from "next/navigation";
import { useEffect, useState } from "react";
import { Menu, Plus, UserRound, X } from "lucide-react";
import { cn } from "@/shared/lib/utils";
import { Logo } from "../ui/logo";
import { routes } from "@/shared/routes";
import { useAuthSession } from "@/features/auth/model/auth-store";

const links = [
  { href: "/", label: "Площадки" },
  { href: "/map", label: "На карте" },
  { href: "/add", label: "Добавить" },
];

export function Header() {
  const pathname = usePathname();
  const [scrolled, setScrolled] = useState(false);
  const [open, setOpen] = useState(false);

  const { isAuthenticated } = useAuthSession();

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 12);
    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  return (
    <header
      className={cn(
        "sticky top-0 z-50 transition-all duration-300",
        scrolled
          ? "border-b border-ink-200/70 bg-cream/70 py-2 backdrop-blur-[18px] backdrop-saturate-[1.4]"
          : "border-b border-transparent py-4",
      )}
    >
      <div className="mx-auto flex max-w-7xl items-center justify-between gap-6 px-5 sm:px-8">
        <Logo />

        <nav className="hidden items-center gap-1 rounded-full border border-ink-200/80 bg-white/60 p-1.5 md:flex">
          {links.map((link) => {
            const active =
              link.href === "/"
                ? pathname === "/"
                : pathname.startsWith(link.href);
            return (
              <Link
                key={link.href}
                href={link.href}
                className={cn(
                  "rounded-full px-4 py-2 text-sm font-semibold transition-colors",
                  active ? "bg-ink text-cream" : "text-ink-500 hover:text-ink",
                )}
              >
                {link.label}
              </Link>
            );
          })}
        </nav>

        <div className="hidden items-center gap-3 md:flex">
          {isAuthenticated ? (
            <Link
              href={routes.profile}
              aria-label="Мой профиль"
              className="grid size-10 place-items-center rounded-full border border-ink-200 bg-white/60 text-ink-500 transition-colors hover:border-ink-300 hover:text-ink"
            >
              <UserRound className="size-4" />
            </Link>
          ) : (
            <Link
              href={routes.authLogin}
              className="rounded-full px-4 py-2.5 text-sm font-semibold text-ink-500 transition-colors hover:text-ink"
            >
              Войти
            </Link>
          )}
          <Link
            href={routes.add}
            className="group inline-flex items-center gap-2 rounded-full bg-ink px-5 py-2.5 text-sm font-semibold text-cream transition-all hover:bg-ink-700 hover:shadow-soft"
          >
            <Plus className="size-4 transition-transform group-hover:rotate-90" />
            Добавить площадку
          </Link>
        </div>

        <button
          onClick={() => setOpen((v) => !v)}
          aria-label="Меню"
          className="grid size-11 place-items-center rounded-full border border-ink-200 bg-white/70 text-ink md:hidden"
        >
          {open ? <X className="size-5" /> : <Menu className="size-5" />}
        </button>
      </div>

      {open && (
        <div className="mx-5 mt-3 animate-fade-up rounded-3xl border border-ink-200 bg-cream p-4 shadow-soft md:hidden">
          <nav className="flex flex-col">
            {links.map((link) => (
              <Link
                key={link.href}
                href={link.href}
                className="rounded-2xl px-4 py-3 text-base font-semibold text-ink-600 hover:bg-ink-100"
              >
                {link.label}
              </Link>
            ))}
          </nav>
          <Link
            href={routes.profile}
            className="mt-1 flex items-center gap-2 rounded-2xl px-4 py-3 text-base font-semibold text-ink-600 hover:bg-ink-100"
          >
            <UserRound className="size-4" />
            Мой профиль
          </Link>
          <div className="mt-3 flex gap-2 border-t border-ink-200 pt-3">
            <Link
              href={routes.authLogin}
              className="flex-1 rounded-2xl border border-ink-200 py-3 text-center text-sm font-semibold"
            >
              Войти
            </Link>
            <Link
              href={routes.add}
              className="flex-1 rounded-2xl bg-ink py-3 text-center text-sm font-semibold text-cream"
            >
              Добавить
            </Link>
          </div>
        </div>
      )}
    </header>
  );
}
