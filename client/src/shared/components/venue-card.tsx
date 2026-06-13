"use client";

import { Star, MapPin, Clock, Lightbulb, CircleDollarSign } from "lucide-react";
import type { Venue } from "@/shared/lib/data";
import { Badge } from "./ui/badge";
import { Card, CardContent } from "./ui/card";
import Image from "next/image";

interface VenueCardProps {
  venue: Venue;
  onClick?: (venue: Venue) => void;
}

export function VenueCard({ venue, onClick }: VenueCardProps) {
  return (
    <Card
      onClick={() => onClick?.(venue)}
      className="group overflow-hidden border-border bg-card hover:border-chart-3/50 transition-all duration-300 cursor-pointer"
    >
      <div className="relative aspect-[16/10] overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-br from-muted to-secondary flex items-center justify-center">
          <span className="text-4xl">
            {venue.sportType === "Баскетбол" && "🏀"}
            {venue.sportType === "Футбол" && "⚽"}
            {venue.sportType === "Теннис" && "🎾"}
            {venue.sportType === "Волейбол" && "🏐"}
            {venue.sportType === "Воркаут" && "💪"}
            {venue.sportType === "Скейтбординг" && "🛹"}
            {venue.sportType === "Хоккей" && "🏒"}
            {venue.sportType === "Бег" && "🏃"}
          </span>
        </div>
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
              className="bg-chart-3/20 text-chart-3 border-0"
            >
              Бесплатно
            </Badge>
          )}
        </div>
        <div className="absolute inset-0 bg-gradient-to-t from-card/90 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity" />
      </div>

      <CardContent className="p-4">
        <div className="flex items-start justify-between gap-2 mb-2">
          <h3 className="font-semibold text-foreground line-clamp-1 group-hover:text-chart-3 transition-colors">
            {venue.name}
          </h3>
          <div className="flex items-center gap-1 shrink-0">
            <Star className="h-4 w-4 fill-chart-3 text-chart-3" />
            <span className="text-sm font-medium text-foreground">
              {venue.rating}
            </span>
            <span className="text-sm text-muted-foreground">
              ({venue.reviewCount})
            </span>
          </div>
        </div>

        <p className="text-sm text-muted-foreground line-clamp-2 mb-3">
          {venue.description}
        </p>

        <div className="flex flex-wrap gap-2 mb-3">
          <Badge variant="outline" className="text-xs">
            {venue.sportType}
          </Badge>
          <Badge variant="outline" className="text-xs">
            {venue.surface}
          </Badge>
        </div>

        <div className="flex flex-col gap-1.5 text-sm text-muted-foreground">
          <div className="flex items-center gap-2">
            <MapPin className="h-3.5 w-3.5 text-chart-3" />
            <span className="line-clamp-1">
              {venue.address}, {venue.city}
            </span>
          </div>
          <div className="flex items-center gap-4">
            <div className="flex items-center gap-2">
              <Clock className="h-3.5 w-3.5 text-chart-3" />
              <span>{venue.workingHours}</span>
            </div>
            {venue.hasLighting && (
              <div className="flex items-center gap-1">
                <Lightbulb className="h-3.5 w-3.5 text-chart-3" />
                <span>Освещение</span>
              </div>
            )}
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
