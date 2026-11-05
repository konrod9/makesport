import { AuthFrame } from "@/shared/components/auth/auth-frame";
import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Вход и регистрация — MakeSport",
  description:
    "Войдите в MakeSport или создайте аккаунт, чтобы сохранять площадки, оставлять отзывы и добавлять новые места на карту.",
};

export default function AuthLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return <AuthFrame>{children}</AuthFrame>;
}
