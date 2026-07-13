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
import { Button } from "@/shared/components/ui/button";
import { ImageGallery } from "@/shared/components/image-gallery";
import { Venue } from "@/entities/venues/types";
import { DialogDescription } from "@radix-ui/react-dialog";

interface VenueDetailsDialogProps {
  venue: Venue | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function VenueDetailsDialog({
  venue,
  open,
  onOpenChange,
}: VenueDetailsDialogProps) {
  if (!venue) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg p-0 overflow-hidden gap-0 border-border bg-card max-h-[90vh] overflow-y-auto">
        <ImageGallery
          images={venue.images}
          title={venue.title}
          sportType={venue.sportType}
          isOpen={venue.isOpen}
          isFree={venue.isFree}
        />

        <div className="p-6 space-y-5">
          <DialogHeader className="space-y-2">
            <div className="flex items-start justify-between gap-3">
              <DialogTitle className="text-xl text-foreground text-balance leading-tight">
                {venue.title}
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
                {venue.address.fullName}
              </p>
              <p className="text-sm text-muted-foreground">
                {venue.address.city}
              </p>
            </div>
          </div>

          <Button className="w-full" size="lg">
            Построить маршрут
          </Button>
        </div>
        <DialogDescription />
      </DialogContent>
    </Dialog>
  );
}
