import { PAGE_SIZE } from "./use-venues-list";
import { create } from "zustand";
import { useShallow } from "zustand/react/shallow";
import { createJSONStorage, persist } from "zustand/middleware";

export type VenuesFilterState = {
  search?: string;
  pageSize: number;
  onlyFree?: boolean;
  onlyOpen?: boolean;
  hasLighting?: boolean;
  city?: string;
  sportType?: string;
  surface?: string;
  // сортировка
};

type Actions = {
  setSearchQuery: (input: VenuesFilterState["search"]) => void;
  setCity: (input: VenuesFilterState["city"]) => void;
  setSportType: (input: VenuesFilterState["sportType"]) => void;
  setSurface: (input: VenuesFilterState["surface"]) => void;
  setOnlyFree: (input: VenuesFilterState["onlyFree"]) => void;
  setOnlyOpen: (input: VenuesFilterState["onlyOpen"]) => void;
  setHasLighting: (input: VenuesFilterState["hasLighting"]) => void;
};

type VenuesFilterStore = VenuesFilterState & Actions;

const initialState: VenuesFilterState = {
  search: "",
  pageSize: PAGE_SIZE,
  onlyFree: false,
  onlyOpen: false,
  hasLighting: false,
  city: "",
  sportType: "",
  surface: "",
};

const useVenuesFilterStore = create<VenuesFilterStore>()(
  persist(
    (set) => ({
      ...initialState,
      setSearchQuery: (input: VenuesFilterState["search"]) =>
        set(() => ({ search: input?.trim() || "" })),
      setCity: (input: VenuesFilterState["city"]) =>
        set(() => ({ city: input?.trim() || "" })),
      setSportType: (input: VenuesFilterState["sportType"]) =>
        set(() => ({ sportType: input?.trim() || "" })),
      setSurface: (input: VenuesFilterState["surface"]) =>
        set(() => ({ surface: input?.trim() || "" })),
      setOnlyFree: (input: VenuesFilterState["onlyFree"]) =>
        set(() => ({ onlyFree: input || false })),
      setOnlyOpen: (input: VenuesFilterState["onlyOpen"]) =>
        set(() => ({ onlyOpen: input || false })),
      setHasLighting: (input: VenuesFilterState["hasLighting"]) =>
        set(() => ({ hasLighting: input || false })),
    }),
    { name: "venues-filter", storage: createJSONStorage(() => localStorage) },
  ),
);

export const useGetVenuesFilter = () => {
  return useVenuesFilterStore(
    useShallow((state) => ({
      search: state.search,
      pageSize: state.pageSize,
      city: state.city,
      sportType: state.sportType,
      surface: state.surface,
      onlyOpen: state.onlyOpen,
      onlyFree: state.onlyFree,
      hasLighting: state.hasLighting,
    })),
  );
};

// TODO: export const useGetVenuesSorting

export const setFilterSearch = (input: VenuesFilterState["search"]) =>
  useVenuesFilterStore.getState().setSearchQuery(input);

export const setFilterCity = (input: VenuesFilterState["city"]) => {
  const { setCity } = useVenuesFilterStore.getState();
  if (input === "all") {
    setCity("");
  } else {
    setCity(input);
  }
};

export const setFilterSportType = (input: VenuesFilterState["sportType"]) => {
  const { setSportType } = useVenuesFilterStore.getState();
  if (input === "all") {
    setSportType("");
  } else {
    setSportType(input);
  }
};

export const setFilterSurface = (input: VenuesFilterState["surface"]) => {
  const { setSurface } = useVenuesFilterStore.getState();
  if (input === "all") {
    setSurface("");
  } else {
    setSurface(input);
  }
};

export const setFilterOnlyOpen = (input: VenuesFilterState["onlyOpen"]) =>
  useVenuesFilterStore.getState().setOnlyOpen(input);

export const setFilterOnlyFree = (input: VenuesFilterState["onlyFree"]) =>
  useVenuesFilterStore.getState().setOnlyFree(input);

export const setFilterHasLighting = (input: VenuesFilterState["hasLighting"]) =>
  useVenuesFilterStore.getState().setHasLighting(input);

export const setInitialFilters = () => {
  setFilterSearch("");
  setFilterCity("");
  setFilterSportType("");
  setFilterSurface("");
  setFilterOnlyFree(false);
  setFilterOnlyOpen(false);
  setFilterHasLighting(false);
};
