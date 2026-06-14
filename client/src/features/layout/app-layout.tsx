"use client";

import { queryClient } from "@/shared/api/query-client";
import { Header } from "@/shared/components/header";
import { QueryClientProvider } from "@tanstack/react-query";

export default function Layout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <QueryClientProvider client={queryClient}>
      <main className="min-h-full flex flex-col">
        <Header />
        {children}
      </main>
    </QueryClientProvider>
  );
}
