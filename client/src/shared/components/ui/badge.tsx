import { cn } from "@/shared/lib/utils";
import { Star } from "lucide-react";

export function Badge({
  children,
  className,
  tone = "neutral",
}: {
  children: React.ReactNode;
  className?: string;
  tone?: "neutral" | "lime" | "dark" | "outline";
}) {
  const tones = {
    neutral: "bg-ink-100 text-ink-600",
    lime: "bg-lime text-ink",
    dark: "bg-ink text-cream",
    outline: "border border-ink-200 bg-white/70 text-ink-600",
  };
  return (
    <span
      className={cn(
        "inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-[11px] font-semibold leading-none",
        tones[tone],
        className,
      )}
    >
      {children}
    </span>
  );
}

export function Rating({
  value,
  reviews,
  className,
}: {
  value: number;
  reviews?: number;
  className?: string;
}) {
  return (
    <span
      className={cn(
        "inline-flex items-center gap-1 text-xs font-semibold",
        className,
      )}
    >
      <Star className="size-3.5 fill-lime-500 text-lime-500" />
      {value.toFixed(1)}
      {reviews !== undefined && (
        <span className="font-medium text-ink-400">({reviews})</span>
      )}
    </span>
  );
}
