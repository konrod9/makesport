import { useRef, useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/shared/components/ui/dialog";
import { Button } from "@/shared/components/ui/button";
import { Upload } from "lucide-react";

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

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Загрузка фотографий</DialogTitle>
          <DialogDescription>
            Перетащите изображения сюда или нажмите для выбора. До 5
            фотографий.
          </DialogDescription>
        </DialogHeader>

        <input
          ref={fileInputRef}
          type="file"
          multiple
          accept="image/*"
          className="hidden"
          onChange={handleFileInputChange}
          aria-label="Выбор файлов"
        />

        <div
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
          onClick={openFilePicker}
          className={`flex flex-col items-center justify-center gap-3 rounded-lg border-2 border-dashed p-10 cursor-pointer transition-colors ${
            isDragging
              ? "border-chart-3 bg-chart-3/10"
              : "border-border hover:border-chart-3/50"
          }`}
        >
          <Upload className="h-8 w-8 text-muted-foreground" />
          <p className="text-sm text-muted-foreground text-center">
            Перетащите изображения сюда
          </p>
          <p className="text-xs text-muted-foreground">или нажмите для выбора</p>
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            type="button"
            onClick={() => onOpenChange(false)}
          >
            Отмена
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
