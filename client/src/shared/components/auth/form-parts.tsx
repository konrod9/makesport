"use client";

import { useState } from "react";
import { Eye, EyeOff, Lock, type LucideIcon } from "lucide-react";
import { cn } from "@/shared/lib/utils";

export const fieldCls =
  "h-12 w-full rounded-2xl border border-ink-200 bg-cream/50 text-sm font-medium text-ink outline-none transition-all placeholder:text-ink-400 focus:border-ink focus:bg-white focus:ring-4 focus:ring-lime/30";

export function Field({
  label,
  hint,
  children,
}: {
  label: string;
  hint?: string;
  children: React.ReactNode;
}) {
  return (
    <label className="block space-y-2">
      <span className="flex items-center gap-1.5 text-[11px] font-semibold uppercase tracking-wider text-ink-500">
        {label}
        {hint && (
          <span className="ml-auto normal-case tracking-normal text-ink-300">
            {hint}
          </span>
        )}
      </span>
      {children}
    </label>
  );
}

export function TextField({
  label,
  icon: Icon,
  hint,
  className,
  ...props
}: React.InputHTMLAttributes<HTMLInputElement> & {
  label: string;
  icon?: LucideIcon;
  hint?: string;
}) {
  return (
    <Field label={label} hint={hint}>
      <div className="relative">
        {Icon && (
          <Icon className="pointer-events-none absolute left-4 top-1/2 size-4 -translate-y-1/2 text-ink-400" />
        )}
        <input
          {...props}
          className={cn(fieldCls, "px-4", Icon && "pl-11", className)}
        />
      </div>
    </Field>
  );
}

export function PasswordField({
  label,
  hint,
  className,
  ...props
}: React.InputHTMLAttributes<HTMLInputElement> & {
  label: string;
  hint?: string;
}) {
  const [visible, setVisible] = useState(false);

  return (
    <Field label={label} hint={hint}>
      <div className="relative">
        <Lock className="pointer-events-none absolute left-4 top-1/2 size-4 -translate-y-1/2 text-ink-400" />
        <input
          {...props}
          type={visible ? "text" : "password"}
          className={cn(fieldCls, "px-4 pl-11 pr-12", className)}
        />
        <button
          type="button"
          onClick={() => setVisible((v) => !v)}
          aria-label={visible ? "Скрыть пароль" : "Показать пароль"}
          className="absolute right-2 top-1/2 grid size-9 -translate-y-1/2 place-items-center rounded-full text-ink-400 transition-colors hover:bg-ink-100 hover:text-ink"
        >
          {visible ? <EyeOff className="size-4" /> : <Eye className="size-4" />}
        </button>
      </div>
    </Field>
  );
}

export function SocialAuth() {
  return (
    <div className="grid grid-cols-2 gap-2.5">
      <button
        type="button"
        className="flex h-12 items-center justify-center gap-2 rounded-2xl border border-ink-200 bg-white text-[13px] font-semibold text-ink transition-all hover:border-ink-300 hover:shadow-card"
      >
        <span className="grid size-6 place-items-center rounded-lg bg-[#0077FF] text-[9px] font-bold text-white">
          VK
        </span>
        ВКонтакте
      </button>
      <button
        type="button"
        className="flex h-12 items-center justify-center gap-2 rounded-2xl border border-ink-200 bg-white text-[13px] font-semibold text-ink transition-all hover:border-ink-300 hover:shadow-card"
      >
        <span className="grid size-6 place-items-center rounded-lg bg-ink text-[10px] font-bold text-lime">
          Я
        </span>
        Яндекс
      </button>
    </div>
  );
}

export function AuthDivider() {
  return (
    <div className="flex items-center gap-3 text-[11px] font-semibold uppercase tracking-wider text-ink-300">
      <span className="h-px flex-1 bg-ink-100" />
      {/* или по e-mail */}
      пока только по e-mail, но в будущем будет и через соц. сети
      <span className="h-px flex-1 bg-ink-100" />
    </div>
  );
}

export function DemoHint({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex items-start gap-2 rounded-2xl bg-cream px-4 py-3 text-[11px] leading-relaxed text-ink-500">
      <span className="mt-1 size-1.5 shrink-0 rounded-full bg-lime-500" />
      {children}
    </div>
  );
}
