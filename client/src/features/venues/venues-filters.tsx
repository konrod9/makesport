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
import {
  setFilterSearch,
  useGetVenuesFilter,
} from "./model/venues-filter-store";
import { FiltersContent } from "@/shared/components/filters-sidebar";
import { useEffect, useState } from "react";
import { useDebounce } from "use-debounce";

export function VenuesFilters(variant?: string) {
  const { search } = useGetVenuesFilter();
  const setSearch = setFilterSearch;

  const [localSearch, setLocalSearch] = useState(search ?? "");
  const [debouncedSearch] = useDebounce(localSearch, 300);

  useEffect(() => {
    setSearch(debouncedSearch);
  }, [debouncedSearch, setSearch]);

  if (variant === "compact") {
    return (
      <div className="space-y-4">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Поиск площадок..."
            value={localSearch}
            onChange={(e) => setLocalSearch(e.target.value)}
            className="pl-9 bg-input border-border"
          />
        </div>
        <div className="p-4 rounded-lg border border-border bg-card">
          {/* <FiltersContent onReset={resetFilters} /> */}
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
            <div className="mt-6">
              {/* <FiltersContent onReset={resetFilters} /> */}
            </div>
          </SheetContent>
        </Sheet>
      </div>
    </>
  );
}
