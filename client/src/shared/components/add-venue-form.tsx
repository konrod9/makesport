"use client";

import { useFileUpload } from "@/entities/file/model/use-file-upload";
import {
  AddressDto,
  CoordinatesDto,
  WorkingHoursDto,
} from "@/entities/venues/types";
import { useCreateVenue } from "@/features/venues/model/use-create-venue";
import dynamic from "next/dynamic";
import { useRouter } from "next/navigation";
import { useCallback, useEffect, useRef, useState } from "react";
import { Controller, useForm, type FieldPath } from "react-hook-form";
import Link from "next/link";
import {
  ArrowLeft,
  Camera,
  Check,
  Clock,
  ImagePlus,
  Info,
  MapPin,
  Sparkles,
  Sun,
  Wallet,
  X,
} from "lucide-react";
import { useAuthSession } from "@/features/auth/model/auth-store";
import { routes } from "@/shared/routes";
import { cn } from "../lib/utils";
import { FormError } from "./ui/form-error";
import { sportTypes, surfaces } from "../lib/data";
import { FileUploadDialog } from "@/entities/file/ui/file-upload-dialog";

type SelectedFile = {
  id: string;
  file: File;
  previewUrl: string;
};

type CreateVenueData = {
  title: string;
  description?: string;
  address: AddressDto;
  coordinates: CoordinatesDto;
  sportType: string;
  surface: string;
  workingHours: WorkingHoursDto;
  hasLighting: boolean;
  isFree: boolean;
  isOpen: boolean;
};

const MapPicker = dynamic(
  () => import("@/shared/components/map-picker").then((m) => m.MapPicker),
  {
    ssr: false,
    loading: () => (
      <div className="h-full w-full flex items-center justify-center bg-card text-muted-foreground">
        Загрузка карты...
      </div>
    ),
  },
);

const steps = ["Основное", "Локация", "Характеристики", "Фото"];
const stepFields: FieldPath<CreateVenueData>[][] = [
  ["title"],
  [
    "address.city",
    "address.street",
    "coordinates.latitude",
    "coordinates.longitude",
  ],
  ["workingHours.workingStart", "workingHours.workingEnd"],
];

export function AddVenueForm() {
  const initialData: CreateVenueData = {
    title: "",
    description: "",
    address: {
      city: "",
      street: "",
      building: "",
      fullName: "",
    },
    coordinates: {
      latitude: 0,
      longitude: 0,
    },
    sportType: "",
    surface: "",
    workingHours: {
      workingStart: "",
      workingEnd: "",
    },
    hasLighting: false,
    isFree: true,
    isOpen: true,
  };

  const router = useRouter();
  const [step, setStep] = useState(0);
  const [isSuccess, setIsSuccess] = useState(false);
  const [selectedFiles, setSelectedFiles] = useState<SelectedFile[]>([]);
  const [isFileUploadDialogOpen, setIsFileUploadDialogOpen] = useState(false);

  const { createVenue, isPending, error, isError } = useCreateVenue();
  const { upload, uploadState } = useFileUpload({ ownerType: "venue" });
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    control,
    setValue,
    trigger,
    getValues,
    clearErrors,
  } = useForm<CreateVenueData>({
    defaultValues: initialData,
    mode: "onTouched",
  });

  const { isAuthenticated, isHydrated } = useAuthSession();

  useEffect(() => {
    if (isHydrated && !isAuthenticated) {
      router.replace(
        `${routes.authLogin}?returnUrl=${encodeURIComponent(routes.add)}`,
      );
    }
  }, [isHydrated, isAuthenticated, router]);

  const onSubmit = (data: CreateVenueData) => {
    createVenue(data, {
      onSuccess: async (result) => {
        const venueId = result.result;

        for (const { file } of selectedFiles) {
          await upload(file, venueId!);
          if (uploadState.status === "error") break;
        }

        // TODO: Не работает. Исправить!
        // Площадка может загрузиться, а файлы не загрузятся. Но при этом площадка сохранится без фотографий + будет написано, что всё прошло успешно
        if (uploadState.status != "error") {
          setIsSuccess(true);
          reset(initialData);
          setTimeout(() => {
            router.push("/venues");
          }, 2000);
        } else {
          console.log("Ошибка загрузки файла");
        }
      },
    });
  };

  const handleNext = async () => {
    const fields = stepFields[step];

    if (fields.length === 0) {
      setStep((s) => s + 1);
      return;
    }

    const ok = await trigger(fields, { shouldFocus: true });
    if (ok) setStep((s) => s + 1);
  };

  const handleCoordsChange = useCallback(
    (coords: CoordinatesDto) => {
      setValue("coordinates.latitude", coords.latitude);
      setValue("coordinates.longitude", coords.longitude);
    },
    [setValue],
  );

  const handleAddressChange = useCallback(
    (address: AddressDto) => {
      setValue("address.city", address.city);
      setValue("address.street", address.street);
      setValue("address.building", address.building ?? "");
      setValue("address.fullName", address.fullName);
    },
    [setValue],
  );

  const selectedFilesRef = useRef<SelectedFile[]>([]);

  useEffect(() => {
    selectedFilesRef.current = selectedFiles;
  }, [selectedFiles]);

  useEffect(() => {
    return () => {
      selectedFilesRef.current.forEach((f) =>
        URL.revokeObjectURL(f.previewUrl),
      );
    };
  }, []);

  const handleFilesSelected = useCallback((files: File[]) => {
    setSelectedFiles((prev) => {
      const remaining = 5 - prev.length;
      if (remaining <= 0) return prev;

      const newFiles = files.slice(0, remaining).map((file) => ({
        id: crypto.randomUUID(),
        file,
        previewUrl: URL.createObjectURL(file),
      }));

      return [...prev, ...newFiles];
    });
  }, []);

  const removeFile = useCallback((id: string) => {
    setSelectedFiles((prev) => {
      const file = prev.find((f) => f.id === id);
      if (file) URL.revokeObjectURL(file.previewUrl);
      return prev.filter((f) => f.id !== id);
    });
  }, []);

  if (isSuccess) {
    return (
      <div className="mx-auto max-w-xl px-5 py-20 text-center">
        <span className="mx-auto grid size-20 place-items-center rounded-full bg-lime">
          <Check className="size-9 text-ink" />
        </span>
        <h1 className="mt-6 font-display text-3xl font-bold">
          Площадка отправлена
        </h1>
        <p className="mt-3 text-ink-500">
          Спасибо! Модератор проверит данные в течение суток — и площадка
          появится на карте.
        </p>
        <div className="mt-8 flex justify-center gap-3">
          <Link
            href="/map"
            className="rounded-full bg-ink px-6 py-3 text-sm font-semibold text-cream hover:bg-ink-700"
          >
            На карту
          </Link>
          <button
            onClick={() => {
              setIsSuccess(false);
              setStep(0);
            }}
            className="rounded-full border border-ink-200 bg-white px-6 py-3 text-sm font-semibold hover:border-ink"
          >
            Добавить ещё
          </button>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) return null;

  return (
    <div className="mx-auto max-w-6xl px-5 py-10 sm:px-8 sm:py-14">
      <Link
        href="/"
        className="inline-flex items-center gap-2 text-sm font-semibold text-ink-400 transition-colors hover:text-ink"
      >
        <ArrowLeft className="size-4" /> Назад к площадкам
      </Link>

      <div className="mt-6 grid gap-8 lg:grid-cols-[1fr_320px] lg:items-start">
        <div>
          <p className="inline-flex items-center gap-2 text-xs font-semibold uppercase tracking-[0.2em] text-lime-600">
            <span className="size-1.5 rounded-full bg-lime-500" />
            Новая точка на карте
          </p>
          <h1 className="mt-3 font-display text-3xl font-bold leading-tight sm:text-4xl">
            Добавить площадку
          </h1>
          <p className="mt-3 max-w-lg text-ink-500">
            Поделитесь местом, где можно тренироваться. Чем точнее данные — тем
            быстрее пройдёт модерация.
          </p>

          <div className="mt-8 flex items-center gap-2">
            {steps.map((label, i) => (
              <div key={label} className="flex flex-1 items-center gap-2">
                <button
                  onClick={() => i < step && setStep(i)}
                  className={cn(
                    "flex items-center gap-2 text-[11px] font-semibold uppercase tracking-wider transition-colors",
                    i === step
                      ? "text-ink"
                      : i < step
                        ? "text-lime-600"
                        : "text-ink-300",
                  )}
                >
                  <span
                    className={cn(
                      "grid size-6 place-items-center rounded-full border text-[10px]",
                      i === step
                        ? "border-ink bg-ink text-cream"
                        : i < step
                          ? "border-lime-500 bg-lime text-ink"
                          : "border-ink-200",
                    )}
                  >
                    {i < step ? <Check className="size-3" /> : i + 1}
                  </span>
                  <span className="hidden sm:inline">{label}</span>
                </button>
                {i < steps.length - 1 && (
                  <span
                    className={cn(
                      "h-px flex-1",
                      i < step ? "bg-lime-500" : "bg-ink-200",
                    )}
                  />
                )}
              </div>
            ))}
          </div>

          <form
            onSubmit={handleSubmit(onSubmit)}
            className="mt-6 rounded-4xl border border-ink-200/70 bg-white p-6 shadow-card sm:p-8"
          >
            {step === 0 && (
              <div className="animate-fade-up space-y-6">
                <SectionTitle
                  icon={<Info className="size-4" />}
                  title="Основная информация"
                  subtitle="Как называется площадка и что здесь можно делать"
                />
                <Field label="Название площадки" required>
                  <input
                    id="name"
                    placeholder="Например, Воркаут-зона в Парке Горького"
                    className={fieldCls(!!errors.title)}
                    {...register("title", { required: "Название обязательно" })}
                  />
                  <FormError message={errors.title?.message} />
                </Field>
                <Field
                  label="Описание"
                  hint="Необязательно, но помогает другим"
                >
                  <textarea
                    id="description"
                    rows={4}
                    placeholder="Расскажите, что есть на площадке: покрытие, инвентарь, особенности..."
                    className={cn(
                      inputCls,
                      "h-auto min-h-[120px] resize-none py-3",
                    )}
                    {...register("description")}
                  />
                </Field>
              </div>
            )}

            {step === 1 && (
              <div className="animate-fade-up space-y-6">
                <SectionTitle
                  icon={<MapPin className="size-4" />}
                  title="Местоположение"
                  subtitle="Поставьте метку на карте и затем уточните адрес при необходимости"
                />
                <div className="mt-4 h-[400px] rounded-lg overflow-hidden border border-border">
                  <MapPicker
                    onCoordinatesChange={handleCoordsChange}
                    onAddressChange={handleAddressChange}
                  />
                  <input
                    type="hidden"
                    {...register("coordinates.latitude", {
                      valueAsNumber: true,
                      validate: (v) => v !== 0 || "Поставьте метку на карте",
                    })}
                  />
                  <input
                    type="hidden"
                    {...register("coordinates.longitude", {
                      valueAsNumber: true,
                      validate: (v) => v !== 0 || "Поставьте метку на карте",
                    })}
                  />
                  <FormError message={errors.coordinates?.latitude?.message} />
                </div>
                <div className="grid gap-4 sm:grid-cols-2">
                  <Field label="Город" required>
                    <input
                      id="city"
                      placeholder="Москва"
                      className={fieldCls(!!errors.address?.city)}
                      {...register("address.city", {
                        required: "Город",
                      })}
                    />
                    <FormError message={errors.address?.city?.message} />
                  </Field>
                  <Field label="Адрес" required>
                    <input
                      id="address"
                      placeholder="Улица, дом"
                      className={fieldCls(!!errors.address?.street)}
                      {...register("address.street", {
                        required: "Улица обязательна",
                      })}
                    />
                    <FormError message={errors.address?.street?.message} />
                  </Field>
                </div>
              </div>
            )}

            {step === 2 && (
              <div className="animate-fade-up space-y-6">
                <SectionTitle
                  icon={<Sparkles className="size-4" />}
                  title="Характеристики"
                  subtitle="Вид спорта, покрытие и условия"
                />
                <Field label="Виды спорта" required>
                  <Controller<CreateVenueData>
                    name="sportType"
                    control={control}
                    rules={{ required: "Вид спорта обязателен" }}
                    render={({ field }) => (
                      <select
                        id="sportType"
                        value={field.value?.toString()}
                        onChange={field.onChange}
                        className={inputCls}
                      >
                        {sportTypes.map((c) => (
                          <option key={c}>{c}</option>
                        ))}
                      </select>
                    )}
                  />
                </Field>
                <Field label="Покрытие" required>
                  <Controller<CreateVenueData>
                    name="surface"
                    control={control}
                    rules={{ required: "Покрытие обязательно" }}
                    render={({ field }) => (
                      <select
                        id="surface"
                        value={field.value?.toString()}
                        onChange={field.onChange}
                        className={inputCls}
                      >
                        {surfaces.map((c) => (
                          <option key={c}>{c}</option>
                        ))}
                      </select>
                    )}
                  />
                </Field>
                <div className="grid gap-4 sm:grid-cols-2">
                  <Field label="Открытие">
                    <div className="relative">
                      <Clock className="pointer-events-none absolute left-3.5 top-1/2 size-4 -translate-y-1/2 text-ink-400" />
                      <input
                        id="workingHoursStart"
                        type="time"
                        className={cn(inputCls, "pl-10")}
                        {...register("workingHours.workingStart")}
                      />
                    </div>
                  </Field>
                  <Field label="Закрытие">
                    <div className="relative">
                      <Clock className="pointer-events-none absolute left-3.5 top-1/2 size-4 -translate-y-1/2 text-ink-400" />
                      <input
                        id="workingHoursEnd"
                        type="time"
                        className={cn(inputCls, "pl-10")}
                        {...register("workingHours.workingEnd")}
                      />
                    </div>
                  </Field>
                </div>
                <div className="grid gap-3 sm:grid-cols-2">
                  <Controller<CreateVenueData>
                    name="hasLighting"
                    control={control}
                    render={({ field }) => (
                      <Toggle
                        icon={<Sun className="size-4" />}
                        label="Есть освещение"
                        hint="Можно заниматься вечером"
                        checked={Boolean(field.value)}
                        onChange={field.onChange}
                      />
                    )}
                  />
                  <Controller<CreateVenueData>
                    name="isFree"
                    control={control}
                    render={({ field }) => (
                      <Toggle
                        icon={<Wallet className="size-4" />}
                        label="Бесплатная площадка"
                        hint="Без оплаты и абонемента"
                        checked={Boolean(field.value)}
                        onChange={field.onChange}
                      />
                    )}
                  />
                </div>
              </div>
            )}

            {step === 3 && (
              <div className="animate-fade-up space-y-6">
                <SectionTitle
                  icon={<Camera className="size-4" />}
                  title="Фотографии"
                  subtitle="До 5 фото — они появятся в карточке площадки"
                />
                <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
                  {selectedFiles.map((file) => (
                    <div
                      key={file.id}
                      className="relative aspect-square rounded-lg overflow-hidden group"
                    >
                      {/* eslint-disable-next-line @next/next/no-img-element */}
                      <img
                        src={file.previewUrl}
                        alt={`Фото`}
                        className="w-full h-full object-cover"
                      />
                      <button
                        type="button"
                        onClick={() => removeFile(file.id)}
                        className="absolute top-1 right-1 p-1 rounded-full bg-background/80 text-foreground opacity-0 group-hover:opacity-100 transition-opacity"
                      >
                        <X className="h-4 w-4" />
                      </button>
                    </div>
                  ))}
                  {selectedFiles.length < 5 && (
                    <button
                      type="button"
                      onClick={() => setIsFileUploadDialogOpen(true)}
                      className="flex aspect-[4/3] flex-col items-center justify-center gap-2 rounded-2xl border-2 border-dashed border-ink-200 text-ink-400 transition-colors hover:border-ink hover:text-ink"
                    >
                      <ImagePlus className="size-6" />
                      <span className="text-[11px] font-semibold">
                        Загрузить
                      </span>
                    </button>
                  )}
                </div>
                <div className="flex items-start gap-2 rounded-2xl bg-cream p-4 text-xs leading-relaxed text-ink-500">
                  <Info className="mt-0.5 size-4 shrink-0" />
                  Загружайте только свои фото или изображения со свободной
                  лицензией.
                </div>
              </div>
            )}

            <div className="mt-8 flex items-center justify-between gap-3 border-t border-ink-100 pt-6">
              <button
                type="button"
                onClick={() => setStep((s) => Math.max(0, s - 1))}
                disabled={step === 0}
                className="inline-flex items-center gap-2 rounded-full px-4 py-3 text-sm font-semibold text-ink-400 transition-colors hover:text-ink disabled:pointer-events-none disabled:opacity-40"
              >
                <ArrowLeft className="size-4" /> Назад
              </button>

              {step < steps.length - 1 ? (
                <button
                  type="button"
                  onClick={handleNext}
                  className="inline-flex items-center gap-2 rounded-full bg-ink px-6 py-3 text-sm font-semibold text-cream transition-all hover:bg-ink-700 disabled:cursor-not-allowed disabled:opacity-40"
                >
                  Далее
                  <svg viewBox="0 0 24 24" className="size-4" fill="none">
                    <path
                      d="M5 12h14M13 6l6 6-6 6"
                      stroke="currentColor"
                      strokeWidth="2"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    />
                  </svg>
                </button>
              ) : (
                <button
                  type="submit"
                  disabled={isPending}
                  className="inline-flex items-center gap-2 rounded-full bg-lime px-6 py-3 text-sm font-bold text-ink transition-all hover:shadow-lime"
                >
                  <Check className="size-4" /> Отправить на модерацию
                </button>
              )}
            </div>
          </form>
          <FileUploadDialog
            open={isFileUploadDialogOpen}
            onOpenChange={setIsFileUploadDialogOpen}
            onFilesSelected={handleFilesSelected}
          />
        </div>

        {/* <aside className="space-y-4 lg:sticky lg:top-24">
          <div className="rounded-4xl border border-ink-200/70 bg-white p-6">
            <h3 className="font-display text-base font-semibold">
              Предпросмотр
            </h3>
            <div className="mt-4 overflow-hidden rounded-3xl border border-ink-100 bg-cream">
              <div className="relative aspect-[4/3] bg-ink-100">
                {photos[0] ? (
                  // eslint-disable-next-line @next/next/no-img-element
                  <img
                    src={photos[0]}
                    alt=""
                    className="size-full object-cover"
                  />
                ) : (
                  <div className="grid size-full place-items-center text-ink-300">
                    <Camera className="size-8" />
                  </div>
                )}
                <div className="absolute left-3 top-3 flex gap-1.5">
                  <span className="rounded-full bg-lime px-2 py-0.5 text-[10px] font-bold text-ink">
                    {free ? "Бесплатно" : "Платно"}
                  </span>
                  {lighting && (
                    <span className="rounded-full bg-white/90 px-2 py-0.5 text-[10px] font-bold text-ink">
                      Свет
                    </span>
                  )}
                </div>
              </div>
              <div className="p-4">
                <p className="line-clamp-1 text-sm font-semibold text-ink">
                  {name || "Название площадки"}
                </p>
                <p className="mt-1 line-clamp-1 text-xs text-ink-400">
                  {address ? `${city}, ${address}` : "Адрес не указан"}
                </p>
                <div className="mt-3 flex flex-wrap gap-1">
                  {(selectedSports.length
                    ? selectedSports
                    : ["Вид спорта"]
                  ).map((s) => (
                    <span
                      key={s}
                      className="rounded-full bg-ink-50 px-2 py-0.5 text-[10px] font-semibold text-ink-500"
                    >
                      {s}
                    </span>
                  ))}
                </div>
                <p className="mt-3 flex items-center gap-1.5 text-[11px] font-semibold text-ink-400">
                  <Clock className="size-3" /> {openTime} — {closeTime}
                  {surface && (
                    <>
                      <span>·</span>
                      {surface}
                    </>
                  )}
                </p>
              </div>
            </div>
          </div>

          <div className="rounded-4xl bg-clay p-6">
            <h3 className="text-sm font-semibold text-ink">
              Что проверяет модератор
            </h3>
            <ul className="mt-3 space-y-2.5">
              {[
                "Метка совпадает с адресом",
                "Площадка существует и доступна",
                "Фото не нарушают правила",
                "Данные о покрытии и спорте верны",
              ].map((t) => (
                <li
                  key={t}
                  className="flex gap-2 text-xs leading-relaxed text-ink-500"
                >
                  <Check className="mt-0.5 size-3.5 shrink-0 text-lime-600" />
                  {t}
                </li>
              ))}
            </ul>
          </div>

          <button
            type="button"
            className="flex w-full items-center justify-center gap-2 rounded-2xl border border-ink-200 bg-white py-3 text-xs font-semibold text-ink-400 transition-colors hover:text-ink"
          >
            <X className="size-3.5" /> Отменить добавление
          </button>
        </aside> */}
      </div>
    </div>
  );
}

const inputCls =
  "h-12 w-full rounded-2xl border border-ink-200 bg-cream/50 px-4 text-sm font-medium text-ink outline-none transition-all placeholder:text-ink-400 focus:border-ink focus:bg-white focus:ring-4 focus:ring-lime/30";

const fieldCls = (invalid?: boolean) =>
  cn(
    inputCls,
    invalid &&
      "border-destructive focus:border-destructive focus:ring-destructive/20",
  );

const photoPool = [
  "https://images.unsplash.com/photo-1529903384028-929ae5dccdf1?auto=format&fit=crop&w=600&q=80",
  "https://images.unsplash.com/photo-1546519638-68e109498ffc?auto=format&fit=crop&w=600&q=80",
  "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?auto=format&fit=crop&w=600&q=80",
  "https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?auto=format&fit=crop&w=600&q=80",
  "https://images.unsplash.com/photo-1520045892732-304bc3ac5d8e?auto=format&fit=crop&w=600&q=80",
];

function SectionTitle({
  icon,
  title,
  subtitle,
}: {
  icon: React.ReactNode;
  title: string;
  subtitle: string;
}) {
  return (
    <div className="flex items-start gap-3">
      <span className="grid size-10 shrink-0 place-items-center rounded-2xl bg-lime text-ink">
        {icon}
      </span>
      <div>
        <h2 className="font-display text-lg font-semibold text-ink">{title}</h2>
        <p className="mt-0.5 text-sm text-ink-400">{subtitle}</p>
      </div>
    </div>
  );
}

function Field({
  label,
  required,
  hint,
  children,
}: {
  label: string;
  required?: boolean;
  hint?: string;
  children: React.ReactNode;
}) {
  return (
    <label className="block space-y-2">
      <span className="flex items-center gap-1.5 text-xs font-semibold uppercase tracking-wider text-ink-500">
        {label}
        {required && <span className="text-lime-600">*</span>}
        {hint && (
          <span className="ml-auto normal-case tracking-normal text-ink-300">
            {hint}
          </span>
        )}
      </span>
      {children}
    </label>
  );
}

function Toggle({
  icon,
  label,
  hint,
  checked,
  onChange,
}: {
  icon: React.ReactNode;
  label: string;
  hint: string;
  checked: boolean;
  onChange: (v: boolean) => void;
}) {
  return (
    <button
      type="button"
      onClick={() => onChange(!checked)}
      className={cn(
        "flex items-center gap-3 rounded-2xl border p-4 text-left transition-all",
        checked
          ? "border-lime-500 bg-lime/15"
          : "border-ink-200 bg-white hover:border-ink-300",
      )}
    >
      <span
        className={cn(
          "grid size-9 shrink-0 place-items-center rounded-xl",
          checked ? "bg-lime text-ink" : "bg-ink-50 text-ink-400",
        )}
      >
        {icon}
      </span>
      <span className="flex-1">
        <span className="block text-[13px] font-semibold text-ink">
          {label}
        </span>
        <span className="block text-[11px] text-ink-400">{hint}</span>
      </span>
      <span
        className={cn(
          "relative h-6 w-11 shrink-0 rounded-full transition-colors",
          checked ? "bg-ink" : "bg-ink-200",
        )}
      >
        <span
          className={cn(
            "absolute top-0.5 size-5 rounded-full bg-white transition-all",
            checked ? "left-[22px]" : "left-0.5",
          )}
        />
      </span>
    </button>
  );
}
