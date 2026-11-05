import { UserRole } from "@/entities/auth/types";
import { Badge } from "../ui/badge";

const roleMeta: Record<UserRole, { label: string }> = {
  Admin: { label: "Администратор" },
  Moderator: { label: "Модератор" },
  User: { label: "Пользователь" },
};

export function RoleBadge({
  role,
  className,
  tone = "neutral",
}: {
  role: UserRole;
  className?: string;
  tone?: "neutral" | "lime" | "dark" | "outline";
}) {
  const meta = roleMeta[role];
  return (
    <Badge tone={tone} className={className}>
      <span className="size-1.5 rounded-full bg-lime" />
      {meta.label}
    </Badge>
  );
}
