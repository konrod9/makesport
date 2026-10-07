"use client";

import { useAuthSession } from "@/features/auth/model/auth-store";
import { useLogout } from "@/features/auth/model/use-logout";
import { useMe } from "@/features/auth/model/use-me";
import { Button } from "@/shared/components/ui/button";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card";
import { FormError } from "@/shared/components/ui/form-error";
import { Spinner } from "@/shared/components/ui/spinner";
import { useRouter } from "next/navigation";
import { Suspense, useEffect } from "react";

function ProfileContent() {
  const router = useRouter();
  const { isAuthenticated, user: sessionUser } = useAuthSession();
  const { logout, isPending: isLoggingOut } = useLogout();
  const meQuery = useMe();

  useEffect(() => {
    if (!isAuthenticated) router.replace("/auth/login?returnUrl=/profile");
  }, [isAuthenticated, router]);

  if (!isAuthenticated) return null;

  if (meQuery.isPending)
    return (
      <div>
        <p>Загрузка профиля...</p>
        <Spinner />
      </div>
    );

  if (meQuery.isError)
    return (
      <div>
        <FormError message="Не удалось загрузить профиль" />
        <Button onClick={() => meQuery.refetch()}>Повторить</Button>
      </div>
    );

  const user = meQuery.data ?? sessionUser;
  if (!user) return null;

  return (
    <div className="mx-auto max-w-md p-4">
      <Card>
        <CardHeader>
          <CardTitle>Профиль</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-col gap-2">
          <p>
            {user.firstName} {user.lastName}
          </p>
          <p className="text-sm text-muted-foreground">{user.email}</p>
          <p className="text-sm">Логин: {user.userName}</p>
          <p className="text-sm">Роль: {user.role}</p>
          <Button
            variant="outline"
            onClick={() => logout()}
            disabled={isLoggingOut}
          >
            {isLoggingOut ? "Выходим..." : "Выйти"}
          </Button>
        </CardContent>
      </Card>
    </div>
  );
}

export default function ProfilePage() {
  return (
    <Suspense
      fallback={
        <div className="flex min-h-screen items-center justify-center p-4">
          <p className="text-sm text-muted-foreground">Загрузка...</p>
        </div>
      }
    >
      <ProfileContent />
    </Suspense>
  );
}
