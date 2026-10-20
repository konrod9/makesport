import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card";
import { AuthUser } from "../types";
import { Label } from "@/shared/components/ui/label";
import { Input } from "@/shared/components/ui/input";
import { Button } from "@/shared/components/ui/button";

export function ProfileEditForm({
  user,
  onCancel,
}: {
  user: AuthUser;
  onCancel: () => void;
}) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Редактировать профиль</CardTitle>
      </CardHeader>
      <CardContent className="flex flex-col gap-4">
        <div className="space-y-2">
          <Label htmlFor="firstName">Имя</Label>
          <Input
            id="firstName"
            placeholder="Введите имя"
            className="bg-input border-border"
            defaultValue={user.firstName}
          ></Input>
        </div>
        <div className="space-y-2">
          <Label htmlFor="lastName">Фамилия</Label>
          <Input
            id="lastName"
            placeholder="Введите фамилию"
            defaultValue={user.lastName}
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="username">Никнейм</Label>
          <Input
            id="username"
            placeholder="Введите новый никнейм"
            defaultValue={user.userName}
          />
        </div>
        <div className="flex flex-col sm:flex-row gap-2">
          <Button size="sm" title="Сохранение скоро появится">
            Сохранить
          </Button>
          <Button variant="outline" size="sm" onClick={onCancel}>
            Отмена
          </Button>
        </div>
        <p className="text-xs text-muted-foreground">
          Редактирование скоро будет доступно
        </p>
      </CardContent>
    </Card>
  );
}
