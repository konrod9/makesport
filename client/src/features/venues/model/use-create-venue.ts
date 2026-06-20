import { venuesApi, venuesQueryOptions } from "@/entities/venues/api";
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
    error: mutation.error, // TODO: Добавить проверку instanceof EnvelopeError
    isPending: mutation.isPending,
  };
}
