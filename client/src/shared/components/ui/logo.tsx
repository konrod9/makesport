import { cn } from "@/shared/lib/utils";
import Link from "next/link";

export function Logo({
  variant = "dark",
  className,
}: {
  variant?: "dark" | "light";
  className?: string;
}) {
  return (
    <Link
      href="/"
      className={cn("group flex items-center gap-2.5", className)}
      aria-label="MakeSport — на главную"
    >
      <span className="relative grid size-10 place-items-center rounded-2xl bg-ink transition-transform duration-300 group-hover:-rotate-6">
        <span className="absolute inset-0 rounded-2xl bg-lime opacity-0 blur-md transition-opacity duration-300 group-hover:opacity-60" />
        <svg
          viewBox="0 0 24 24"
          className="relative size-5 text-lime"
          fill="none"
        >
          <path
            d="M12 21s-7-5.2-7-11a7 7 0 1 1 14 0c0 5.8-7 11-7 11Z"
            stroke="currentColor"
            strokeWidth="1.8"
            strokeLinejoin="round"
          />
          <circle
            cx="12"
            cy="10"
            r="2.6"
            stroke="currentColor"
            strokeWidth="1.8"
          />
        </svg>
      </span>
      <span
        className={cn(
          "font-display text-lg font-bold tracking-tight",
          variant === "light" ? "text-cream" : "text-ink",
        )}
      >
        Make
        <span className={variant === "light" ? "text-lime" : "text-lime-600"}>
          Sport
        </span>
      </span>
    </Link>
  );
}
