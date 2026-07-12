"use client";

import { useState, useEffect, useCallback, useRef } from "react";
import Image from "next/image";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { Badge } from "@/shared/components/ui/badge";
import { cn } from "@/shared/lib/utils";
import type { MediaDto } from "@/entities/venues/types";

interface ImageGalleryProps {
  images: MediaDto[];
  title: string;
  sportType: string;
  isOpen: boolean;
  isFree: boolean;
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

export function ImageGallery({
  images,
  title,
  sportType,
  isOpen,
  isFree,
}: ImageGalleryProps) {
  const [currentIndex, setCurrentIndex] = useState(0);
  const [imageError, setImageError] = useState(false);
  const touchStartX = useRef(0);
  const touchEndX = useRef(0);

  const hasImages = images.length > 0;
  const hasMultiple = images.length > 1;
  const currentImage = images[currentIndex];

  const goTo = useCallback((index: number) => {
    setCurrentIndex(index);
    setImageError(false);
  }, []);

  const goNext = useCallback(() => {
    setCurrentIndex((prev) => (prev + 1) % images.length);
    setImageError(false);
  }, [images.length]);

  const goPrev = useCallback(() => {
    setCurrentIndex((prev) => (prev - 1 + images.length) % images.length);
    setImageError(false);
  }, [images.length]);

  useEffect(() => {
    if (!hasMultiple) return;

    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "ArrowLeft") {
        e.preventDefault();
        goPrev();
      }
      if (e.key === "ArrowRight") {
        e.preventDefault();
        goNext();
      }
    };

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [hasMultiple, goNext, goPrev]);

  useEffect(() => {
    setCurrentIndex(0);
    setImageError(false);
  }, [images]);

  const handleTouchStart = (e: React.TouchEvent) => {
    touchStartX.current = e.touches[0].clientX;
  };

  const handleTouchMove = (e: React.TouchEvent) => {
    touchEndX.current = e.touches[0].clientX;
  };

  const handleTouchEnd = () => {
    if (!hasMultiple) return;
    const diff = touchStartX.current - touchEndX.current;
    const threshold = 50;
    if (Math.abs(diff) > threshold) {
      if (diff > 0) goNext();
      else goPrev();
    }
  };

  const showFallback = !hasImages || !currentImage?.url;

  return (
    <div
      className="relative aspect-[16/9] bg-muted overflow-hidden group select-none"
      onTouchStart={handleTouchStart}
      onTouchMove={handleTouchMove}
      onTouchEnd={handleTouchEnd}
    >
      {showFallback || imageError ? (
        <div className="absolute inset-0 bg-gradient-to-br from-muted to-secondary flex items-center justify-center">
          <span className="text-6xl">{getSportEmoji(sportType)}</span>
        </div>
      ) : (
        <Image
          key={currentImage.id}
          src={currentImage.url}
          alt={`${title} — фото ${currentIndex + 1}`}
          fill
          className="object-cover"
          unoptimized
          sizes="512px"
          onError={() => setImageError(true)}
        />
      )}

      <div className="absolute top-3 left-3 flex gap-2 z-10">
        <Badge
          variant={isOpen ? "default" : "secondary"}
          className={
            isOpen ? "bg-green-600 hover:bg-green-600 text-foreground" : ""
          }
        >
          {isOpen ? "Открыто" : "Закрыто"}
        </Badge>
        {isFree && (
          <Badge
            variant="secondary"
            className="bg-primary/20 text-primary border-0"
          >
            Бесплатно
          </Badge>
        )}
      </div>

      {hasMultiple && !showFallback && (
        <>
          <button
            onClick={goPrev}
            className="absolute left-2 top-1/2 -translate-y-1/2 rounded-full bg-background/80 p-1.5 opacity-0 group-hover:opacity-100 transition-opacity hover:bg-background z-10"
            aria-label="Предыдущее фото"
          >
            <ChevronLeft className="h-5 w-5" />
          </button>
          <button
            onClick={goNext}
            className="absolute right-2 top-1/2 -translate-y-1/2 rounded-full bg-background/80 p-1.5 opacity-0 group-hover:opacity-100 transition-opacity hover:bg-background z-10"
            aria-label="Следующее фото"
          >
            <ChevronRight className="h-5 w-5" />
          </button>

          <div className="absolute bottom-3 left-1/2 -translate-x-1/2 flex gap-1.5 z-10">
            {images.map((_, index) => (
              <button
                key={index}
                onClick={() => goTo(index)}
                className={cn(
                  "h-2 rounded-full transition-all",
                  index === currentIndex
                    ? "bg-background w-3"
                    : "bg-background/50 hover:bg-background/80 w-2",
                )}
                aria-label={`Фото ${index + 1}`}
              />
            ))}
          </div>
        </>
      )}
    </div>
  );
}
