import { useRef, useState, useEffect } from "react";
import { createPortal } from "react-dom";
import { Upload, X } from "lucide-react";

type Props = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onFilesSelected: (files: File[]) => void;
};

export function FileUploadDialog({
  open,
  onOpenChange,
  onFilesSelected,
}: Props) {
  const [isDragging, setIsDragging] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  // Закрытие по Esc
  useEffect(() => {
    if (!open) return;
    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") onOpenChange(false);
    };
    document.addEventListener("keydown", onKeyDown);
    return () => document.removeEventListener("keydown", onKeyDown);
  }, [open, onOpenChange]);

  // Блокировка скролла фона, пока диалог открыт
  useEffect(() => {
    if (!open) return;
    const original = document.body.style.overflow;
    document.body.style.overflow = "hidden";
    return () => {
      document.body.style.overflow = original;
    };
  }, [open]);

  if (!open) return null;

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(true);
  };

  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);

    const files = Array.from(e.dataTransfer.files).filter((f) =>
      f.type.startsWith("image/"),
    );
    if (files.length > 0) {
      onFilesSelected(files);
      onOpenChange(false);
    }
  };

  const handleFileInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.target.files ?? []);
    if (files.length > 0) {
      onFilesSelected(files);
      onOpenChange(false);
    }
    e.target.value = "";
  };

  const openFilePicker = () => {
    fileInputRef.current?.click();
  };

  return createPortal(
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      {/* Оверлей */}
      <div
        className="absolute inset-0 bg-ink/40 backdrop-blur-sm"
        onClick={() => onOpenChange(false)}
        aria-hidden="true"
      />

      {/* Само модальное окно */}
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby="upload-dialog-title"
        className="relative w-full max-w-md overflow-hidden rounded-4xl border border-ink-200/70 bg-white p-6 shadow-card sm:p-8 animate-fade-up"
      >
        {/* Шапка */}
        <div className="flex items-start justify-between gap-4">
          <div>
            <h2
              id="upload-dialog-title"
              className="font-display text-lg font-semibold text-ink"
            >
              Загрузка фотографий
            </h2>
            <p className="mt-1 text-sm text-ink-500">
              Перетащите изображения сюда или нажмите для выбора. До 5
              фотографий.
            </p>
          </div>
          <button
            type="button"
            onClick={() => onOpenChange(false)}
            className="grid size-9 shrink-0 place-items-center rounded-full text-ink-400 transition-colors hover:bg-ink-50 hover:text-ink"
            aria-label="Закрыть"
          >
            <X className="size-4" />
          </button>
        </div>

        {/* Скрытый input */}
        <input
          ref={fileInputRef}
          type="file"
          multiple
          accept="image/*"
          className="hidden"
          onChange={handleFileInputChange}
          aria-label="Выбор файлов"
        />

        {/* Зона drag & drop */}
        <div
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
          onClick={openFilePicker}
          className={`mt-6 flex cursor-pointer flex-col items-center justify-center gap-3 rounded-3xl border-2 border-dashed p-10 text-center transition-colors ${
            isDragging
              ? "border-lime bg-lime/10"
              : "border-ink-200 bg-cream/50 hover:border-lime/50"
          }`}
        >
          <span className="grid size-12 place-items-center rounded-2xl bg-lime text-ink">
            <Upload className="size-5" />
          </span>
          <div>
            <p className="text-sm font-semibold text-ink">
              Перетащите изображения сюда
            </p>
            <p className="mt-0.5 text-xs text-ink-400">
              или нажмите, чтобы выбрать файлы
            </p>
          </div>
        </div>

        {/* Футер с кнопкой */}
        <div className="mt-6 flex justify-end border-t border-ink-100 pt-5">
          <button
            type="button"
            onClick={() => onOpenChange(false)}
            className="rounded-full border border-ink-200 bg-white px-5 py-2.5 text-sm font-semibold text-ink-500 transition-colors hover:border-ink-300 hover:text-ink"
          >
            Отмена
          </button>
        </div>
      </div>
    </div>,
    document.body,
  );
}
