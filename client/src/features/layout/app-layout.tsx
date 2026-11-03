"use client";

import { queryClient } from "@/shared/api/query-client";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { QueryClientProvider } from "@tanstack/react-query";
import { Header } from "@/shared/components/layout/header";
import { Footer } from "@/shared/components/layout/footer";

export default function Layout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <QueryClientProvider client={queryClient}>
      <Header />
      <main>{children}</main>
      <Footer />
      <ReactQueryDevtools initialIsOpen={false} position="bottom" />
    </QueryClientProvider>
  );
}
