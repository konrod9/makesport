import { LoginForm } from "@/shared/components/auth/login-form";
import { Metadata } from "next";
import { Suspense } from "react";

export const metadata: Metadata = {
  title: "Вход — MakeSport",
};

export default function LoginPage() {
  return (
    <Suspense
      fallback={
        <div className="flex min-h-screen items-center justify-center p-4">
          <p className="text-sm text-muted-foreground">Загрузка...</p>
        </div>
      }
    >
      <LoginForm />
    </Suspense>
  );
}
