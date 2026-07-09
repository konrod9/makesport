import { useRef, useState } from "react";
import { OwnerType } from "../types";
import { fileApi, PartETag, StartMultipartUploadRequest } from "../api";

export type Props = {
  ownerType: OwnerType;
  onSuccess?: (mediaAsssetId: string) => Promise<void>;
};

export type UploadProgress = {
  status: "idle" | "uploading" | "completed" | "error";
  error?: string;
};

export function useFileUpload({ ownerType, onSuccess }: Props) {
  const [uploadState, setUploadState] = useState<UploadProgress>({
    status: "idle",
  });

  const upload = async (file: File, ownerId: string) => {
    try {
      setUploadState({ status: "uploading" });

      const startUploadRequest: StartMultipartUploadRequest = {
        fileName: file.name,
        contentType: file.type,
        size: file.size,
        assetType: "image",
        ownerId: ownerId,
        ownerType: ownerType,
      };

      const { mediaAssetId, uploadId, chunkUploadUrls, chunkSize } =
        await fileApi.startMultipartUpload(startUploadRequest);

      const partETags: PartETag[] = [];

      const totalChunks = chunkUploadUrls.length;

      for (let i = 0; i < totalChunks; i++) {
        const chunkInfo = chunkUploadUrls[i];
        const start = i * chunkSize;
        const end = Math.min(file.size, start + chunkSize);

        const chunk = file.slice(start, end);

        const eTag = await fileApi.uploadChunk(chunkInfo.uploadUrl, chunk);

        partETags.push({
          partNumber: chunkInfo.partNumber,
          eTag,
        });
      }

      await fileApi.completeMultipartUpload({
        mediaAssetId,
        uploadId,
        partETags,
      });

      setUploadState((prev) => ({ ...prev, status: "completed" }));
    } catch (error) {
      setUploadState({
        status: "error",
        error: (error as Error).message,
      });
    }
  };

  return { upload, uploadState };
}
