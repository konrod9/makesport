"use client";

import { Search, SlidersHorizontal, X } from "lucide-react";
import { Input } from "@/shared/components/ui/input";
import { Button } from "@/shared/components/ui/button";
import { Label } from "@/shared/components/ui/label";
import { Checkbox } from "@/shared/components/ui/checkbox";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/shared/components/ui/sheet";
import { cities, sportTypes, surfaces } from "@/shared/lib/data";

interface FiltersProps {
  searchQuery: string;
  setSearchQuery: (value: string) => void;
  selectedCity: string;
  setSelectedCity: (value: string) => void;
  selectedSport: string;
  setSelectedSport: (value: string) => void;
  selectedSurface: string;
  setSelectedSurface: (value: string) => void;
  onlyFree: boolean;
  setOnlyFree: (value: boolean) => void;
  onlyOpen: boolean;
  setOnlyOpen: (value: boolean) => void;
  hasLighting: boolean;
  setHasLighting: (value: boolean) => void;
  onReset: () => void;
  variant?: "sidebar" | "compact" | "sheet";
}

export function FiltersContent({
  selectedCity,
  setSelectedCity,
  selectedSport,
  setSelectedSport,
  selectedSurface,
  setSelectedSurface,
  onlyFree,
  setOnlyFree,
  onlyOpen,
  setOnlyOpen,
  hasLighting,
  setHasLighting,
  onReset,
  hideTitle = false,
}: Omit<FiltersProps, "searchQuery" | "setSearchQuery"> & {
  hideTitle?: boolean;
}) {
  const hasActiveFilters =
    selectedCity !== "all" ||
    selectedSport !== "all" ||
    selectedSurface !== "all" ||
    onlyFree ||
    onlyOpen ||
    hasLighting;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        {!hideTitle && (
          <h3 className="font-semibold text-foreground">Фильтры</h3>
        )}
        {hasActiveFilters && (
          <Button
            variant="ghost"
            size="sm"
            onClick={onReset}
            className="text-muted-foreground hover:text-foreground ml-auto"
          >
            <X className="h-4 w-4 mr-1" />
            Сбросить
          </Button>
        )}
      </div>

      <div className="space-y-4">
        <div className="space-y-2">
          <Label htmlFor="city" className="text-sm text-muted-foreground">
            Город
          </Label>
          <Select value={selectedCity} onValueChange={setSelectedCity}>
            <SelectTrigger id="city" className="bg-input border-border">
              <SelectValue placeholder="Выберите город" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Все города</SelectItem>
              {cities.map((city) => (
                <SelectItem key={city} value={city}>
                  {city}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div className="space-y-2">
          <Label htmlFor="sport" className="text-sm text-muted-foreground">
            Вид спорта
          </Label>
          <Select value={selectedSport} onValueChange={setSelectedSport}>
            <SelectTrigger id="sport" className="bg-input border-border">
              <SelectValue placeholder="Выберите вид спорта" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Все виды спорта</SelectItem>
              {sportTypes.map((sport) => (
                <SelectItem key={sport} value={sport}>
                  {sport}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div className="space-y-2">
          <Label htmlFor="surface" className="text-sm text-muted-foreground">
            Покрытие
          </Label>
          <Select value={selectedSurface} onValueChange={setSelectedSurface}>
            <SelectTrigger id="surface" className="bg-input border-border">
              <SelectValue placeholder="Выберите покрытие" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Любое покрытие</SelectItem>
              {surfaces.map((surface) => (
                <SelectItem key={surface} value={surface}>
                  {surface}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="space-y-3 pt-2">
        <div className="flex items-center space-x-2">
          <Checkbox
            id="free"
            checked={onlyFree}
            onCheckedChange={(checked) => setOnlyFree(checked as boolean)}
          />
          <Label
            htmlFor="free"
            className="text-sm text-foreground cursor-pointer"
          >
            Только бесплатные
          </Label>
        </div>
        <div className="flex items-center space-x-2">
          <Checkbox
            id="open"
            checked={onlyOpen}
            onCheckedChange={(checked) => setOnlyOpen(checked as boolean)}
          />
          <Label
            htmlFor="open"
            className="text-sm text-foreground cursor-pointer"
          >
            Сейчас открыто
          </Label>
        </div>
        <div className="flex items-center space-x-2">
          <Checkbox
            id="lighting"
            checked={hasLighting}
            onCheckedChange={(checked) => setHasLighting(checked as boolean)}
          />
          <Label
            htmlFor="lighting"
            className="text-sm text-foreground cursor-pointer"
          >
            Есть освещение
          </Label>
        </div>
      </div>
    </div>
  );
}

export function FiltersSidebar(props: FiltersProps) {
  const {
    searchQuery,
    setSearchQuery,
    variant = "sidebar",
    ...filterProps
  } = props;

  if (variant === "compact") {
    return (
      <div className="space-y-4">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Поиск площадок..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="pl-9 bg-input border-border"
          />
        </div>
        <div className="p-4 rounded-lg border border-border bg-card">
          <FiltersContent {...filterProps} />
        </div>
      </div>
    );
  }

  return (
    <>
      {/* Desktop Sidebar */}
      <aside className="hidden lg:block w-72 shrink-0">
        <div className="sticky top-24 space-y-6">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Поиск площадок..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="pl-9 bg-input border-border"
            />
          </div>
          <div className="p-4 rounded-lg border border-border bg-card">
            <FiltersContent {...filterProps} />
          </div>
        </div>
      </aside>

      {/* Mobile Filters */}
      <div className="lg:hidden flex gap-3 mb-6">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Поиск площадок..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="pl-9 bg-input border-border"
          />
        </div>
        <Sheet>
          <SheetTrigger asChild>
            <Button variant="outline" size="icon">
              <SlidersHorizontal className="h-4 w-4" />
            </Button>
          </SheetTrigger>
          <SheetContent side="left" className="w-80">
            <SheetHeader>
              <SheetTitle>Фильтры</SheetTitle>
            </SheetHeader>
            <div className="mt-6">
              <FiltersContent {...filterProps} />
            </div>
          </SheetContent>
        </Sheet>
      </div>
    </>
  );
}
