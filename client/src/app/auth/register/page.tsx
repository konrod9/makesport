"use client";

import { useForm } from "react-hook-form";
import { useRouter, useSearchParams } from "next/navigation";
import Link from "next/link";
import { Input } from "@/shared/components/ui/input";
import { Label } from "@/shared/components/ui/label";
import { Button } from "@/shared/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card";
import { FormError } from "@/shared/components/ui/form-error";
import { useAuthSession } from "@/features/auth/model/auth-store";
import { Suspense, useEffect } from "react";
import { useRegister } from "@/features/auth/model/use-register";

type RegisterFormData = {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
};

function RegisterForm() {
  const router = useRouter();
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
    formState: { errors },
  } = useForm<RegisterFormData>({
    defaultValues: { email: "", password: "", firstName: "", lastName: "" },
  });

  const onSubmit = (data: RegisterFormData) => {
    signUp(
      {
        email: data.email.trim(),
        password: data.password,
        firstName: data.firstName.trim(),
        lastName: data.lastName.trim(),
      },
      { onSuccess: () => router.push(returnUrl) },
    );
  };

  return (
    <div className="flex min-h-screen items-center justify-center p-4">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Регистрация</CardTitle>
          <CardDescription>Создайте аккаунт MakeSport</CardDescription>
        </CardHeader>
        <CardContent>
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="flex flex-col gap-4"
          >
            <div className="flex flex-col gap-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                autoComplete="email"
                {...rhfRegister("email", {
                  required: "Email обязателен",
                  pattern: {
                    value: /^\S+@\S+\.\S+$/,
                    message: "Некорректный email",
                  },
                })}
              />
              <FormError message={errors.email?.message} />
            </div>
            <div className="flex flex-col gap-2">
              <Label htmlFor="password">Пароль</Label>
              <Input
                id="password"
                type="password"
                autoComplete="current-password"
                {...rhfRegister("password", {
                  required: "Пароль обязателен",
                  minLength: { value: 8, message: "Минимум 8 символов" },
                  maxLength: { value: 100, message: "Максимум 100 символов" },
                })}
              />
              <FormError message={errors.password?.message} />
            </div>
            <div className="flex flex-col gap-2">
              <Label htmlFor="firstName">Имя</Label>
              <Input
                id="firstName"
                type="text"
                autoComplete="given-name"
                {...rhfRegister("firstName", {
                  required: "Имя обязательно",
                  minLength: { value: 1, message: "Введите корректное имя" },
                  maxLength: { value: 50, message: "Максимум 50 символов" },
                })}
              />
              <FormError message={errors.firstName?.message} />
            </div>
            <div className="flex flex-col gap-2">
              <Label htmlFor="lastName">Фамилия</Label>
              <Input
                id="lastName"
                type="text"
                autoComplete="family-name"
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
            </div>
            {/* серверная ошибка (401 auth.failed и др.) */}
            <FormError message={error?.firstMessage} />
            <Button type="submit" disabled={isPending}>
              {isPending ? "Регистрация..." : "Создать аккаунт"}
            </Button>
            <p className="text-sm text-muted-foreground">
              Есть аккаунт?{" "}
              <Link href="/auth/login" className="underline">
                Войти
              </Link>
            </p>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}

export default function RegisterPage() {
  return (
    <Suspense
      fallback={
        <div className="flex min-h-screen items-center justify-center p-4">
          <p className="text-sm text-muted-foreground">Загрузка...</p>
        </div>
      }
    >
      <RegisterForm />
    </Suspense>
  );
}
