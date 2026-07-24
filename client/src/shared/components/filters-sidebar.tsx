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
import { sportTypes, surfaces } from "@/shared/lib/data";
import {
  setFilterCity,
  setFilterHasLighting,
  setFilterOnlyFree,
  setFilterOnlyOpen,
  setFilterSearch,
  setFilterSportType,
  setFilterSurface,
  setInitialFilters,
  useGetVenuesFilter,
} from "@/features/venues/model/venues-filter-store";
import { useCities } from "@/features/venues/model/use-cities";

interface FiltersProps {
  variant?: "sidebar" | "compact" | "sheet";
}

export function FiltersContent({ hideTitle = false }: { hideTitle?: boolean }) {
  const { city, sportType, surface, onlyFree, onlyOpen, hasLighting } =
    useGetVenuesFilter();

  const { data } = useCities();

  const hasActiveFilters =
    city !== undefined ||
    sportType !== undefined ||
    surface !== undefined ||
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
            onClick={setInitialFilters}
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
          <Select value={city} onValueChange={setFilterCity}>
            <SelectTrigger id="city" className="bg-input border-border">
              <SelectValue placeholder="Выберите город" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Все города</SelectItem>
              {data?.result?.map((city) => (
                <SelectItem key={city.name} value={city.name}>
                  {city.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div className="space-y-2">
          <Label htmlFor="sport" className="text-sm text-muted-foreground">
            Вид спорта
          </Label>
          <Select value={sportType} onValueChange={setFilterSportType}>
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
          <Select value={surface} onValueChange={setFilterSurface}>
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
            onCheckedChange={(checked) => setFilterOnlyFree(checked as boolean)}
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
            onCheckedChange={(checked) => setFilterOnlyOpen(checked as boolean)}
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
            onCheckedChange={(checked) =>
              setFilterHasLighting(checked as boolean)
            }
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
  const { variant = "sidebar" } = props;
  const { search } = useGetVenuesFilter();

  if (variant === "compact") {
    return (
      <div className="space-y-4">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Поиск площадок..."
            value={search ?? ""}
            onChange={(e) => setFilterSearch(e.target.value)}
            className="pl-9 bg-input border-border"
          />
        </div>
        <div className="p-4 rounded-lg border border-border bg-card">
          <FiltersContent />
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
              value={search}
              onChange={(e) => setFilterSearch(e.target.value)}
              className="pl-9 bg-input border-border"
            />
          </div>
          <div className="p-4 rounded-lg border border-border bg-card">
            <FiltersContent />
          </div>
        </div>
      </aside>

      {/* Mobile Filters */}
      <div className="lg:hidden flex gap-3 mb-6">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Поиск площадок..."
            value={search}
            onChange={(e) => setFilterSearch(e.target.value)}
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
            <div className="ml-6">
              <FiltersContent />
            </div>
          </SheetContent>
        </Sheet>
      </div>
    </>
  );
}
