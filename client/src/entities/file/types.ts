/**
 * Owner types - entities that can own files
 */
export type OwnerType = "venue" | "profile";

/**
 * Asset types supported by FileService
 * Must match backend FileService.Domain.AssetType enum
 */
export type AssetType = "video" | "preview" | "avatar" | "image";
