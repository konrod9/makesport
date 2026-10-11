"use client";

import { MyVenues } from "@/entities/auth/ui/my-venues";
import { ProfileEditForm } from "@/entities/auth/ui/profile-edit-form";
import { ProfileHero } from "@/entities/auth/ui/profile-hero";
import { ProfileInfo } from "@/entities/auth/ui/profile-info";
import { ProfileStats } from "@/entities/auth/ui/profile-stats";
import { useAuthSession } from "@/features/auth/model/auth-store";
import { useLogout } from "@/features/auth/model/use-logout";
import { useMe } from "@/features/auth/model/use-me";
import { Button } from "@/shared/components/ui/button";
import { FormError } from "@/shared/components/ui/form-error";
import { Spinner } from "@/shared/components/ui/spinner";
import { useRouter } from "next/navigation";
import { Suspense, useEffect, useState } from "react";

function ProfileContent() {
  const router = useRouter();
  const { isAuthenticated, isHydrated, user: sessionUser } = useAuthSession();
  const { logout, isPending: isLoggingOut } = useLogout();
  const [isEditing, setIsEditing] = useState(false);
  const meQuery = useMe();

  useEffect(() => {
    if (isHydrated && !isAuthenticated)
      router.replace("/auth/login?returnUrl=/profile");
  }, [isHydrated, isAuthenticated, router]);

  if (!isAuthenticated) return null;

  if (!isHydrated)
    return (
      <div className="flex min-h-[50vh] items-center justify-center p-4">
        <div className="flex flex-col items-center gap-3">
          <Spinner />
          <p className="text-sm text-muted-foreground">Загрузка...</p>
        </div>
      </div>
    );

  if (!isAuthenticated) return null;

  if (meQuery.isPending)
    return (
      <div className="flex min-h-[50vh] items-center justify-center p-4">
        <div className="flex flex-col items-center gap-3">
          <Spinner />
          <p className="text-sm text-muted-foreground">Загрузка профиля...</p>
        </div>
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
    <div className="mx-auto flex w-full max-w-2xl flex-col gap-4 p-4 sm:px-6">
      <ProfileHero
        user={user}
        isLoggingOut={isLoggingOut}
        onLogout={() => logout()}
        onEdit={() => setIsEditing((v) => !v)}
      />
      {isEditing ? (
        <ProfileEditForm user={user} onCancel={() => setIsEditing(false)} />
      ) : (
        <ProfileInfo user={user} />
      )}
      <ProfileStats />
      <MyVenues />
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
