import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card";
import { Heart, Mail, MapPin, MessageSquare } from "lucide-react";

function StatCell({
  icon: Icon,
  value,
  label,
}: {
  icon: typeof Mail;
  value: string;
  label: string;
}) {
  return (
    <div className="flex flex-col items-center gap-1 py-2 text-center">
      <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-muted">
        <Icon className="h-4 w-4 text-muted-foreground" />
      </div>
      <p className="text-lg font-bold leading-none">{value}</p>
      <p className="text-xs text-muted-foreground">{label}</p>
    </div>
  );
}

export function ProfileStats() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Статистика</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="grid grid-cols-3 gap-2">
          <StatCell icon={MapPin} value="0" label="Площадок добавлено" />
          <StatCell icon={MessageSquare} value="0" label="Отзывов" />
          <StatCell icon={Heart} value="0" label="В избранном" />
        </div>
        <p className="mt-2 text-center text-xs text-muted-foreground">
          Статистика скоро появится
        </p>
      </CardContent>
    </Card>
  );
}
