import { Button } from "@/shared/components/ui/button";
import Link from "next/link";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card";
import { MapPin } from "lucide-react";
import { routes } from "@/shared/routes";
import { IconPlus } from "@/shared/components/ui/iconPlus";

export function MyVenues() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Мои площадки</CardTitle>
      </CardHeader>
      <CardContent className="flex flex-col items-center gap-2 py-4 text-center">
        <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-muted">
          <MapPin className="h-5 w-5 text-muted-foreground" />
        </div>
        <p className="text-lg font-bold">Вы пока не добавили площадок</p>
        <p className="text-sm text-muted-foreground">
          Здесь появятся Ваши площадки
        </p>
        <Button size="sm" asChild>
          <Link href={routes.add}>
            <IconPlus className="h-4 w-4" />
            Добавить
          </Link>
        </Button>
      </CardContent>
    </Card>
  );
}
