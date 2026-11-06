"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import {
  ArrowRight,
  AtSign,
  Check,
  Mail,
  MapPin,
  UserRound,
} from "lucide-react";
import {
  AuthDivider,
  DemoHint,
  Field,
  PasswordField,
  SocialAuth,
  TextField,
  fieldCls,
} from "./form-parts";
import { cn } from "@/shared/lib/utils";
import { useAuthSession } from "@/features/auth/model/auth-store";
import { useRegister } from "@/features/auth/model/use-register";
import { useForm } from "react-hook-form";
import { FormError } from "../ui/form-error";

const strengthLabels = [
  "Слишком короткий",
  "Простой",
  "Средний",
  "Надёжный",
  "Отличный",
];

type RegisterFormData = {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  userName: string;
};

export function RegisterForm() {
  const router = useRouter();
  const [terms, setTerms] = useState(false);
  const [pending, setPending] = useState(false);

  const searchParams = useSearchParams();
  const returnUrl = searchParams.get("returnUrl") ?? "/";
  const { isAuthenticated } = useAuthSession();
  const { register: signUp, isPending, error } = useRegister();

  useEffect(() => {
    if (isAuthenticated) router.replace(returnUrl);
  }, [isAuthenticated, returnUrl, router]);

  const {
    register: rhfRegister,
    handleSubmit,
    getValues,
    formState: { errors },
  } = useForm<RegisterFormData>({
    defaultValues: {
      email: "",
      password: "",
      firstName: "",
      lastName: "",
      userName: "",
    },
  });

  const onSubmit = (data: RegisterFormData) => {
    signUp(
      {
        email: data.email.trim(),
        password: data.password,
        firstName: data.firstName.trim(),
        lastName: data.lastName.trim(),
        userName: data.userName.trim(),
      },
      { onSuccess: () => router.push(returnUrl) },
    );
  };

  const checks = [
    getValues("password").length >= 8,
    /[A-ZА-Я]/.test(getValues("password")),
    /\d/.test(getValues("password")),
    /[^A-Za-zА-Яа-я0-9]/.test(getValues("password")),
  ];
  const score = checks.filter(Boolean).length;

  return (
    <div className="mx-auto w-full max-w-md">
      <p className="inline-flex items-center gap-2 text-[11px] font-semibold uppercase tracking-[0.2em] text-lime-600">
        <span className="size-1.5 rounded-full bg-lime-500" />
        Присоединяйтесь
      </p>
      <h1 className="mt-3 font-display text-3xl font-bold leading-tight tracking-tight text-ink sm:text-[2rem]">
        Создать аккаунт
      </h1>
      <p className="mt-2 text-sm text-ink-500">
        Пара минут — и ваша карта спорта начнёт заполняться.
      </p>

      {/* <div className="mt-6">
        <SocialAuth />
      </div> */}

      <div className="my-5">
        <AuthDivider />
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-3.5">
        <TextField
          label="Введите имя"
          icon={UserRound}
          autoComplete="firstName"
          required
          minLength={2}
          placeholder="Алексей"
          {...rhfRegister("firstName", {
            required: "Имя обязательно",
            minLength: { value: 1, message: "Введите корректное имя" },
            maxLength: { value: 50, message: "Максимум 50 символов" },
          })}
        />
        <TextField
          label="Фамилия"
          icon={UserRound}
          autoComplete="lastName"
          required
          minLength={2}
          placeholder="Смирнов"
          {...rhfRegister("lastName", {
            required: "Фамилия обязательна",
            minLength: {
              value: 1,
              message: "Введите корректную фамилию",
            },
            maxLength: { value: 50, message: "Максимум 50 символов" },
          })}
        />
        <FormError message={errors.lastName?.message} />

        <TextField
          label="Логин"
          icon={AtSign}
          autoComplete="userName"
          required
          minLength={4}
          placeholder="Логин"
          {...rhfRegister("userName", {
            required: "Логин обязателен",
            minLength: {
              value: 4,
              message: "Минимальная длина 4 символа",
            },
            maxLength: { value: 50, message: "Максимум 50 символов" },
            pattern: {
              value: /^[A-Za-z][A-Za-z0-9]*$/,
              message:
                "Логин должен начинаться с буквы и содержать только английские буквы и цифры",
            },
          })}
        />
        <FormError message={errors.userName?.message} />

        <TextField
          label="E-mail"
          icon={Mail}
          type="email"
          autoComplete="email"
          required
          placeholder="you@example.com"
          {...rhfRegister("email", {
            required: "Email обязателен",
            pattern: {
              value: /^\S+@\S+\.\S+$/,
              message: "Некорректный email",
            },
          })}
        />

        {/* <Field label="Город">
          <div className="relative">
            <MapPin className="pointer-events-none absolute left-4 top-1/2 size-4 -translate-y-1/2 text-ink-400" />
            <select
              value={city}
              onChange={(e) => setCity(e.target.value)}
              className={cn(fieldCls, "appearance-none px-4 pl-11")}
            >
              {cities.map((c) => (
                <option key={c}>{c}</option>
              ))}
            </select>
            <svg
              viewBox="0 0 24 24"
              className="pointer-events-none absolute right-4 top-1/2 size-4 -translate-y-1/2 text-ink-400"
              fill="none"
            >
              <path
                d="m6 9 6 6 6-6"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          </div>
        </Field> */}

        <div className="space-y-2">
          <PasswordField
            label="Пароль"
            autoComplete="new-password"
            required
            minLength={8}
            placeholder="Минимум 8 символов"
            {...rhfRegister("password", { required: "Пароль обязателен" })}
          />
          {getValues("password").length > 0 && (
            <div className="animate-fade-in px-1">
              <div className="flex gap-1">
                {[0, 1, 2, 3].map((i) => (
                  <span
                    key={i}
                    className={cn(
                      "h-1 flex-1 rounded-full transition-colors",
                      i < score ? "bg-lime-500" : "bg-ink-100",
                    )}
                  />
                ))}
              </div>
              <p className="mt-1.5 flex items-center justify-between text-[10px] font-medium text-ink-400">
                <span>{strengthLabels[score]}</span>
                <span>Буквы, цифры и символы</span>
              </p>
            </div>
          )}
        </div>

        <label className="flex cursor-pointer items-start gap-2.5 pt-1 text-xs leading-relaxed text-ink-500">
          <input
            type="checkbox"
            required
            checked={terms}
            onChange={(e) => setTerms(e.target.checked)}
            className="mt-0.5 size-4 shrink-0 rounded border-ink-300 accent-ink"
          />
          <span>
            Согласен с{" "}
            <Link
              href="#"
              className="font-semibold text-ink underline decoration-lime-400 decoration-2 underline-offset-2"
            >
              условиями использования
            </Link>{" "}
            и{" "}
            <Link
              href="#"
              className="font-semibold text-ink underline decoration-lime-400 decoration-2 underline-offset-2"
            >
              политикой конфиденциальности
            </Link>
          </span>
        </label>

        <button
          type="submit"
          disabled={!terms || pending}
          className="group inline-flex h-12 w-full items-center justify-center gap-2 rounded-full bg-lime px-6 text-sm font-bold text-ink transition-all hover:shadow-lime disabled:cursor-not-allowed disabled:opacity-40"
        >
          {pending ? (
            <>
              <span className="size-4 animate-spin rounded-full border-2 border-ink/30 border-t-ink" />
              Создаём аккаунт…
            </>
          ) : (
            <>
              <Check className="size-4" />
              Создать аккаунт
              <ArrowRight className="size-4 transition-transform group-hover:translate-x-1" />
            </>
          )}
        </button>
      </form>

      <p className="mt-5 text-center text-sm text-ink-400">
        Уже есть аккаунт?{" "}
        <Link
          href="/auth/login"
          className="font-semibold text-ink underline decoration-lime decoration-2 underline-offset-4 hover:text-lime-600"
        >
          Войти
        </Link>
      </p>
    </div>
  );
}
