import { apiClient } from "@/shared/api/axios-instance";
import { AssetType, OwnerType } from "./types";
import { Envelope } from "@/shared/api/envelope";
import axios from "axios";

export type StartMultipartUploadRequest = {
  fileName: string;
  assetType: AssetType;
  contentType: string;
  size: number;
  ownerId: string;
  ownerType: OwnerType;
};

export type StartMultipartUploadResponse = {
  mediaAssetId: string;
  uploadId: string;
  chunkUploadUrls: ChunkUploadUrl[];
  chunkSize: number;
};

export type ChunkUploadUrl = {
  partNumber: number;
  uploadUrl: string;
};

export type CompleteMultipartUploadRequest = {
  mediaAssetId: string;
  uploadId: string;
  partETags: PartETag[];
};

export type PartETag = {
  partNumber: number;
  eTag: string;
};

export const fileApi = {
  /**
   * Start multipart upload session
   */
  startMultipartUpload: async (
    request: StartMultipartUploadRequest,
    signal?: AbortSignal,
  ): Promise<StartMultipartUploadResponse> => {
    const response = await apiClient.post<
      Envelope<StartMultipartUploadResponse>
    >("/files/multipart-upload", request);

    return response.data.result!;
  },

  /**
   * Upload a single chunk to presigned URL
   */
  uploadChunk: async (
    uploadUrl: string,
    chunk: Blob,
    signal?: AbortSignal,
  ): Promise<string> => {
    const response = await axios.put(uploadUrl, chunk, { signal });

    const eTag = response.headers["etag"].replace(/"/g, "") || "";
    return eTag;
  },

  /**
   * Complete multipart upload
   */
  completeMultipartUpload: async (
    request: CompleteMultipartUploadRequest,
  ): Promise<void> => {
    await apiClient.post("/files/complete-upload", request);
  },
};
