import { RegisterForm } from "@/shared/components/auth/register-form";
import { Suspense } from "react";

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
