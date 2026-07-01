import { venuesApi, venuesQueryOptions } from "@/entities/venues/api";
import { EnvelopeError } from "@/shared/api/errors";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export function useCreateVenue() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: venuesApi.createVenue,
    onSettled: () =>
      queryClient.invalidateQueries({
        queryKey: [venuesQueryOptions.baseKey],
      }),
    // TODO: Добавить onError и onSuccess
  });

  return {
    createVenue: mutation.mutate,
    isError: mutation.isError,
    error: mutation.error instanceof EnvelopeError ? mutation.error : undefined,
    isPending: mutation.isPending,
  };
}
