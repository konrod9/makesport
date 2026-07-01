import { cn } from "@/shared/lib/utils";
import { LoaderIcon } from "lucide-react";

function Spinner({ className, ...props }: React.ComponentProps<"svg">) {
  return (
    <LoaderIcon
      strokeWidth={2}
      data-slot="spinner"
      role="status"
      aria-label="Loading"
      className={cn("size-6 animate-spin", className)}
      {...props}
    />
  );
}

export { Spinner };
