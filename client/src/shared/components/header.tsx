"use client";

import Link from "next/link";
import { Button } from "@/shared/components/ui/button";
import { useState } from "react";
import { routes } from "@/shared/routes";
import { useAuthSession } from "@/features/auth/model/auth-store";
import { useLogout } from "@/features/auth/model/use-logout";

function IconMapPin({ className }: { className?: string }) {
  return (
    <svg
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      className={className}
      aria-hidden="true"
    >
      <path d="M20 10c0 5-8 11-8 11s-8-6-8-11a8 8 0 1 1 16 0Z" />
      <circle cx="12" cy="10" r="3" />
    </svg>
  );
}

function IconPlus({ className }: { className?: string }) {
  return (
    <svg
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      className={className}
      aria-hidden="true"
    >
      <path d="M12 5v14" />
      <path d="M5 12h14" />
    </svg>
  );
}

function IconMenu({ className }: { className?: string }) {
  return (
    <svg
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      className={className}
      aria-hidden="true"
    >
      <path d="M4 6h16" />
      <path d="M4 12h16" />
      <path d="M4 18h16" />
    </svg>
  );
}

function IconX({ className }: { className?: string }) {
  return (
    <svg
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      className={className}
      aria-hidden="true"
    >
      <path d="M18 6 6 18" />
      <path d="M6 6l12 12" />
    </svg>
  );
}

export function Header() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  const { isAuthenticated, user } = useAuthSession();
  const { logout, isPending: isLoggingOut } = useLogout();
  const initials = user
    ? `${user.firstName[0] ?? ""}${user.lastName[0] ?? ""}`.toUpperCase()
    : "";

  return (
    <header className="sticky top-0 z-50 border-b border-border bg-background/95 backdrop-blur supports-backdrop-filter:bg-background/60">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="flex h-16 items-center justify-between">
          <Link href={routes.home} className="flex items-center gap-2">
            <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-chart-3">
              <IconMapPin className="h-5 w-5 text-primary-foreground" />
            </div>
            <span className="text-xl font-bold text-foreground">MakeSport</span>
          </Link>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center gap-6">
            <Link
              href="/"
              className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors"
            >
              Площадки
            </Link>
            <Link
              href="/map"
              className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors"
            >
              На карте
            </Link>
            <Link
              href="/add"
              className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors"
            >
              Добавить площадку
            </Link>
          </nav>

          <div className="hidden md:flex items-center gap-3">
            {isAuthenticated && user ? (
              <>
                <Link
                  href={routes.profile}
                  className="flex items-center gap-2 text-sm font-medium"
                  title="Профиль"
                >
                  <span className="flex h-8 w-8 items-center justify-center rounded-full bg-chart-3 text-xs font-bold text-primary-foreground">
                    {initials}
                  </span>
                  <span className="max-w-32 truncate">
                    {user.firstName} {user.lastName}
                  </span>
                </Link>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => logout()}
                  disabled={isLoggingOut}
                >
                  {isLoggingOut ? "Выходим..." : "Выйти"}
                </Button>
              </>
            ) : (
              <Button variant="outline" size="sm" asChild>
                <Link href={routes.authLogin}>Войти</Link>
              </Button>
            )}
            <Button size="sm" asChild>
              <Link href={routes.add} className="flex items-center">
                <IconPlus className="mr-1.5 h-4 w-4" />
                Добавить
              </Link>
            </Button>
          </div>

          {/* Mobile Menu Button */}
          <button
            className="md:hidden p-2 text-muted-foreground hover:text-foreground"
            onClick={() => setIsMenuOpen(!isMenuOpen)}
          >
            {isMenuOpen ? (
              <IconX className="h-6 w-6" />
            ) : (
              <IconMenu className="h-6 w-6" />
            )}
          </button>
        </div>

        {/* Mobile Navigation */}
        {isMenuOpen && (
          <div className="md:hidden py-4 border-t border-border">
            <nav className="flex flex-col gap-3">
              <Link
                href="/"
                className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors px-2 py-2"
                onClick={() => setIsMenuOpen(false)}
              >
                Площадки
              </Link>
              <Link
                href="/map"
                className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors px-2 py-2"
                onClick={() => setIsMenuOpen(false)}
              >
                На карте
              </Link>
              <Link
                href="/add"
                className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors px-2 py-2"
                onClick={() => setIsMenuOpen(false)}
              >
                Добавить площадку
              </Link>
              <div className="flex flex-col gap-2 pt-2">
                {isAuthenticated && user ? (
                  <>
                    <Link
                      href={routes.profile}
                      className="flex items-center gap-2 px-2 py-2 text-sm font-medium"
                      onClick={() => setIsMenuOpen(false)}
                    >
                      <span className="flex h-8 w-8 items-center justify-center rounded-full bg-chart-3 text-xs font-bold text-primary-foreground">
                        {initials}
                      </span>
                      {user.firstName} {user.lastName}
                    </Link>
                    <Button
                      variant="outline"
                      size="sm"
                      className="w-full"
                      onClick={() => {
                        setIsMenuOpen(false);
                        logout();
                      }}
                      disabled={isLoggingOut}
                    >
                      {isLoggingOut ? "Выходим..." : "Выйти"}
                    </Button>
                  </>
                ) : (
                  <Button
                    variant="outline"
                    size="sm"
                    className="w-full"
                    asChild
                  >
                    <Link
                      href={routes.authLogin}
                      onClick={() => setIsMenuOpen(false)}
                    >
                      Войти
                    </Link>
                  </Button>
                )}
                <Button size="sm" className="w-full" asChild>
                  <Link
                    href={routes.add}
                    className="flex items-center justify-center"
                    onClick={() => setIsMenuOpen(false)}
                  >
                    <IconPlus className="mr-1.5 h-4 w-4" />
                    Добавить
                  </Link>
                </Button>
              </div>
            </nav>
          </div>
        )}
      </div>
    </header>
  );
}
