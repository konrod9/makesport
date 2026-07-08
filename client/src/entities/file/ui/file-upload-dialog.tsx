import { useRef, useState } from "react";
import { AssetType, OwnerType } from "../types";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/shared/components/ui/dialog";

type Props = {
  ownerType: OwnerType;
  assetType: AssetType;
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

export function FileUploadDialog({
  ownerType,
  assetType,
  open,
  onOpenChange,
}: Props) {
  const [isDragging, setIsDragging] = useState(false);

  const fileInputRef = useRef<HTMLInputElement>(null);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>{`Загрузка фотографий`}</DialogTitle>
          <DialogDescription />
        </DialogHeader>

        <input
          ref={fileInputRef}
          type="file"
          accept={getAcceptString(assetType)}
          className="hidden"
          onChange={handleFileInputChange}
          aria-label={`Выбор файла: ${config.label}`}
        />

        {/* <div className="space-y-4">
          {isIdle && (
            <DropZone
              isDragging={isDragging}
              onDragOver={handleDragOver}
              onDragLeave={handleDragLeave}
              onDrop={handleDrop}
              onClick={openFilePicker}
              label={`Перетащите изображения сюда`}
              description={config.description}
            />
          )}

          {isUploading && (
            <UploadingState
              fileName={uploadState.fileName}
              fileSize={uploadState.fileSize}
              progress={uploadState.progress}
              uploadedBytes={uploadState.uploadedBytes}
              onCancel={cancel}
              totalBytes={uploadState.totalBytes}
            />
          )}

          {isFailed && <ErrorState error={error} onRetry={reset} />}

          {isCompleted && <CompletedState fileName={uploadState.fileName} />}
        </div> */}

        <DialogFooter>
          {/* {isIdle && (
            <Button variant="outline" onClick={() => onOpenChange(false)}>
              Отмена
            </Button>
          )}
          {isCompleted && (
            <Button onClick={() => onOpenChange(false)}>Готово</Button>
          )} */}
        </DialogFooter>
      </DialogContent>
    </Dialog>
}
