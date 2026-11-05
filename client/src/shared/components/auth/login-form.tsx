"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import { ArrowRight, Info, Mail } from "lucide-react";
import { AuthDivider, PasswordField, TextField } from "./form-parts";
import { useAuthSession } from "@/features/auth/model/auth-store";
import { useLogin } from "@/features/auth/model/use-login";
import { useForm, useWatch } from "react-hook-form";
import { FormError } from "../ui/form-error";

type LoginFormData = {
  email: string;
  password: string;
};

export function LoginForm() {
  const router = useRouter();
  const [remember, setRemember] = useState(true);
  const [recovery, setRecovery] = useState(false);

  const searchParams = useSearchParams();
  const returnUrl = searchParams.get("returnUrl") ?? "/";
  const { isAuthenticated, isHydrated } = useAuthSession();
  const { login, isPending, error } = useLogin();

  useEffect(() => {
    if (isHydrated && isAuthenticated) router.replace(returnUrl);
  }, [isHydrated, isAuthenticated, returnUrl, router]);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<LoginFormData>({
    defaultValues: { email: "", password: "" },
  });

  const email = useWatch({ control, name: "email" });

  const onSubmit = (data: LoginFormData) => {
    login(
      { email: data.email.trim(), password: data.password },
      { onSuccess: () => router.push(returnUrl) },
    );
  };

  return (
    <div className="mx-auto w-full max-w-md">
      <p className="inline-flex items-center gap-2 text-[11px] font-semibold uppercase tracking-[0.2em] text-lime-600">
        <span className="size-1.5 rounded-full bg-lime-500" />С возвращением
      </p>
      <h1 className="mt-3 font-display text-3xl font-bold leading-tight tracking-tight text-ink sm:text-[2rem]">
        Вход в аккаунт
      </h1>
      <p className="mt-2 text-sm text-ink-500">
        Продолжите собирать свою карту спорта.
      </p>

      {/* <div className="mt-7">
        <SocialAuth />
      </div> */}

      <div className="my-6">
        <AuthDivider />
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <TextField
          label="E-mail"
          icon={Mail}
          type="email"
          autoComplete="email"
          required
          placeholder="you@example.com"
          {...register("email", {
            required: "Email обязателен",
            pattern: {
              value: /^\S+@\S+\.\S+$/,
              message: "Некорректный email",
            },
          })}
        />
        <FormError message={errors.email?.message} />

        <PasswordField
          label="Пароль"
          autoComplete="current-password"
          required
          minLength={8}
          placeholder="Минимум 8 символов"
          {...register("password", { required: "Пароль обязателен" })}
        />
        <FormError message={errors.password?.message} />
        <FormError message={error?.firstMessage} />

        <div className="flex flex-wrap items-center justify-between gap-3 pt-1">
          <label className="flex cursor-pointer items-center gap-2 text-xs font-medium text-ink-500">
            <input
              type="checkbox"
              checked={remember}
              onChange={(e) => setRemember(e.target.checked)}
              className="size-4 rounded border-ink-300 accent-ink"
            />
            Запомнить меня
          </label>
          <button
            type="button"
            onClick={() => setRecovery(true)}
            className="text-xs font-semibold text-ink-500 underline decoration-lime-400 decoration-2 underline-offset-4 transition-colors hover:text-ink"
          >
            Забыли пароль?
          </button>
        </div>

        {recovery && (
          <p className="flex items-start gap-2 rounded-2xl bg-lime/15 px-4 py-3 text-xs leading-relaxed text-ink-600">
            <Info className="mt-0.5 size-4 shrink-0 text-lime-600" />
            Демо-режим: ссылка для восстановления отправлена на{" "}
            <span className="font-semibold">{email || "вашу почту"}</span>.
          </p>
        )}

        <button
          type="submit"
          disabled={isPending}
          className="group inline-flex h-12 w-full items-center justify-center gap-2 rounded-full bg-lime px-6 text-sm font-bold text-ink transition-all hover:shadow-lime disabled:cursor-wait disabled:opacity-70"
        >
          {isPending ? (
            <>
              <span className="size-4 animate-spin rounded-full border-2 border-ink/30 border-t-ink" />
              Входим…
            </>
          ) : (
            <>
              Войти
              <ArrowRight className="size-4 transition-transform group-hover:translate-x-1" />
            </>
          )}
        </button>
      </form>

      <p className="mt-6 text-center text-sm text-ink-400">
        Нет аккаунта?{" "}
        <Link
          href="/auth/register"
          className="font-semibold text-ink underline decoration-lime decoration-2 underline-offset-4 hover:text-lime-600"
        >
          Зарегистрироваться
        </Link>
      </p>
    </div>
  );
}
