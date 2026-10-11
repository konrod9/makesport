import { Badge } from "@/shared/components/ui/badge";
import { UserRole } from "../types";

const roleMeta: Record<
  UserRole,
  { label: string; variant: "default" | "secondary" | "outline" }
> = {
  Admin: { label: "Администратор", variant: "default" },
  Moderator: { label: "Модератор", variant: "secondary" },
  User: { label: "Пользователь", variant: "outline" },
};

export function RoleBadge({ role }: { role: UserRole }) {
  const meta = roleMeta[role];
  return <Badge variant={meta.variant}>{meta.label}</Badge>;
}
