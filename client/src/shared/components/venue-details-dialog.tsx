"use client";

import {
  Star,
  MapPin,
  Clock,
  Lightbulb,
  CircleDollarSign,
  Building2,
  Layers,
} from "lucide-react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/shared/components/ui/dialog";
import { Badge } from "@/shared/components/ui/badge";
import { Button } from "@/shared/components/ui/button";
import type { Venue } from "@/shared//lib/data";

interface VenueDetailsDialogProps {
  venue: Venue | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const getSportEmoji = (sportType: string) => {
  switch (sportType) {
    case "Баскетбол":
      return "🏀";
    case "Футбол":
      return "⚽";
    case "Теннис":
      return "🎾";
    case "Волейбол":
      return "🏐";
    case "Воркаут":
      return "💪";
    case "Скейтбординг":
      return "🛹";
    case "Хоккей":
      return "🏒";
    case "Бег":
      return "🏃";
    default:
      return "📍";
  }
};

export function VenueDetailsDialog({
  venue,
  open,
  onOpenChange,
}: VenueDetailsDialogProps) {
  if (!venue) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg p-0 overflow-hidden gap-0 border-border bg-card max-h-[90vh] overflow-y-auto">
        {/* Hero image */}
        <div className="relative aspect-[16/9] bg-gradient-to-br from-muted to-secondary flex items-center justify-center">
          <span className="text-6xl">{getSportEmoji(venue.sportType)}</span>
          <div className="absolute top-3 left-3 flex gap-2">
            <Badge
              variant={venue.isOpen ? "default" : "secondary"}
              className={
                venue.isOpen
                  ? "bg-green-600 hover:bg-green-600 text-foreground"
                  : ""
              }
            >
              {venue.isOpen ? "Открыто" : "Закрыто"}
            </Badge>
            {venue.isFree && (
              <Badge
                variant="secondary"
                className="bg-primary/20 text-primary border-0"
              >
                Бесплатно
              </Badge>
            )}
          </div>
        </div>

        <div className="p-6 space-y-5">
          <DialogHeader className="space-y-2">
            <div className="flex items-start justify-between gap-3">
              <DialogTitle className="text-xl text-foreground text-balance leading-tight">
                {venue.name}
              </DialogTitle>
              <div className="flex items-center gap-1 shrink-0 rounded-md bg-secondary px-2 py-1">
                <Star className="h-4 w-4 fill-chart-3 text-chart-3" />
                <span className="text-sm font-semibold text-foreground">
                  {venue.rating}
                </span>
                <span className="text-sm text-muted-foreground">
                  ({venue.reviewCount})
                </span>
              </div>
            </div>
          </DialogHeader>

          <p className="text-sm text-muted-foreground leading-relaxed">
            {venue.description}
          </p>

          {/* Key facts grid */}
          <div className="grid grid-cols-2 gap-3">
            <div className="flex items-center gap-2.5 rounded-lg bg-secondary/50 p-3">
              <Building2 className="h-4 w-4 text-chart-3 shrink-0" />
              <div className="min-w-0">
                <p className="text-xs text-muted-foreground">Вид спорта</p>
                <p className="text-sm font-medium text-foreground truncate">
                  {venue.sportType}
                </p>
              </div>
            </div>
            <div className="flex items-center gap-2.5 rounded-lg bg-secondary/50 p-3">
              <Layers className="h-4 w-4 text-chart-3 shrink-0" />
              <div className="min-w-0">
                <p className="text-xs text-muted-foreground">Покрытие</p>
                <p className="text-sm font-medium text-foreground truncate">
                  {venue.surface}
                </p>
              </div>
            </div>
            <div className="flex items-center gap-2.5 rounded-lg bg-secondary/50 p-3">
              <Clock className="h-4 w-4 text-chart-3 shrink-0" />
              <div className="min-w-0">
                <p className="text-xs text-muted-foreground">Время работы</p>
                <p className="text-sm font-medium text-foreground truncate">
                  {venue.workingHours}
                </p>
              </div>
            </div>
            <div className="flex items-center gap-2.5 rounded-lg bg-secondary/50 p-3">
              {venue.isFree ? (
                <CircleDollarSign className="h-4 w-4 text-chart-3 shrink-0" />
              ) : (
                <CircleDollarSign className="h-4 w-4 text-chart-3 shrink-0" />
              )}
              <div className="min-w-0">
                <p className="text-xs text-muted-foreground">Стоимость</p>
                <p className="text-sm font-medium text-foreground truncate">
                  {venue.isFree ? "Бесплатно" : "Платно"}
                </p>
              </div>
            </div>
          </div>

          {/* Lighting */}
          <div className="flex items-center gap-2.5 rounded-lg bg-secondary/50 p-3">
            <Lightbulb
              className={`h-4 w-4 shrink-0 ${venue.hasLighting ? "text-chart-3" : "text-muted-foreground"}`}
            />
            <p className="text-sm text-foreground">
              {venue.hasLighting
                ? "Есть освещение для вечерних игр"
                : "Освещение отсутствует"}
            </p>
          </div>

          {/* Address */}
          <div className="flex items-start gap-2.5 rounded-lg border border-border p-3">
            <MapPin className="h-4 w-4 text-chart-3 shrink-0 mt-0.5" />
            <div>
              <p className="text-sm font-medium text-foreground">
                {venue.address}
              </p>
              <p className="text-sm text-muted-foreground">{venue.city}</p>
            </div>
          </div>

          <Button className="w-full" size="lg">
            Построить маршрут
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
