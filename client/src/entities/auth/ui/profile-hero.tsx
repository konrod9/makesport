import type { AuthUser } from "@/entities/auth/types";
import { Button } from "@/shared/components/ui/button";
import { Card, CardContent } from "@/shared/components/ui/card";
import { Pencil } from "lucide-react";
import { RoleBadge } from "./role-badge";

function getInitials(firstName: string, lastName: string) {
  return `${firstName[0] ?? ""}${lastName[0] ?? ""}`.toUpperCase();
}

type ProfileHeroProps = {
  user: AuthUser;
  isLoggingOut: boolean;
  onLogout: () => void;
  onEdit: () => void;
};

export function ProfileHero({
  user,
  isLoggingOut,
  onLogout,
  onEdit,
}: ProfileHeroProps) {
  return (
    <Card>
      <CardContent className="flex flex-col gap-4 p-6 sm:flex-row sm:items-center">
        <div className="flex h-16 w-16 shrink-0 items-center justify-center rounded-full bg-chart-3 text-lg font-bold text-primary-foreground">
          {getInitials(user.firstName, user.lastName)}
        </div>

        <div className="min-w-0 flex-1">
          <h2 className="truncate text-xl font-bold">
            {user.firstName} {user.lastName}
          </h2>
          <p className="truncate text-sm text-muted-foreground">{user.email}</p>
          <div className="mt-2 flex items-center gap-2">
            <RoleBadge role={user.role} />
            <span className="truncate text-xs text-muted-foreground">
              @{user.userName}
            </span>
          </div>
        </div>

        <div className="flex shrink-0 gap-2 max-sm:flex-col max-sm:[&>*]:w-full sm:items-center">
          <Button variant="outline" size="sm" onClick={onEdit}>
            <Pencil />
            Редактировать
          </Button>
          <Button
            variant="outline"
            size="sm"
            onClick={onLogout}
            disabled={isLoggingOut}
          >
            {isLoggingOut ? "Выходим..." : "Выйти"}
          </Button>
        </div>
      </CardContent>
    </Card>
  );
}
