import { AtSign, Check, Copy, Hash, Mail, ShieldCheck } from "lucide-react";
import React, { useState } from "react";
import { AuthUser } from "../types";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card";
import { RoleBadge } from "./role-badge";
import { Button } from "@/shared/components/ui/button";

function InfoRow({
  icon: Icon,
  label,
  children,
}: {
  icon: typeof Mail;
  label: string;
  children: React.ReactNode;
}) {
  return (
    <div className="flex items-center gap-3 py-2">
      <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-muted">
        <Icon className="h-4 w-4 text-muted-foreground" />
      </div>
      <div className="min-w-0 flex-1">
        <p className="text-xs text-muted-foreground">{label}</p>
        <div className="truncate text-sm font-medium">{children}</div>
      </div>
    </div>
  );
}

export function ProfileInfo({ user }: { user: AuthUser }) {
  const [copied, setCopied] = useState(false);

  const copyId = async () => {
    try {
      await navigator.clipboard.writeText(user.id);
      setCopied(true);
      setTimeout(() => setCopied(false), 1500);
    } catch {}
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle>Информация</CardTitle>
      </CardHeader>
      <CardContent className="divide-y divide-border">
        <InfoRow icon={Mail} label="Email">
          {user.email}
        </InfoRow>
        <InfoRow icon={AtSign} label="Логин">
          @{user.userName}
        </InfoRow>
        <InfoRow icon={ShieldCheck} label="Роль">
          <RoleBadge role={user.role} />
        </InfoRow>
        <div className="flex items-center gap-3 py-2">
          <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-muted">
            <Hash className="h-4 w-4 text-muted-foreground" />
          </div>
          <div className="min-w-0 flex-1">
            <p className="text-xs text-muted-foreground">ID пользователя</p>
            <p className="truncate font-mono text-xs">{user.id}</p>
          </div>
          <Button
            variant="ghost"
            size="icon-sm"
            onClick={copyId}
            title="Скопировать ID"
          >
            {copied ? <Check /> : <Copy />}
          </Button>
        </div>
      </CardContent>
    </Card>
  );
}
