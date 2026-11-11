import type { Metadata } from "next";
import { Manrope, Unbounded } from "next/font/google";
import "./globals.css";
import Layout from "@/features/layout/app-layout";

const manrope = Manrope({
  subsets: ["latin", "cyrillic"],
  variable: "--font-manrope",
  display: "swap",
});

const unbounded = Unbounded({
  subsets: ["latin", "cyrillic"],
  variable: "--font-unbounded",
  display: "swap",
  weight: ["400", "500", "600", "700", "800"],
});

export const metadata: Metadata = {
  title: "MakeSport — спортивные площадки рядом с тобой",
  description:
    "Краудсорсинговая платформа для поиска, добавления и оценки спортивных площадок. Находите бесплатные площадки на карте своего города.",
};
export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      lang="en"
      className={`${manrope.variable} ${unbounded.variable}`}
      suppressHydrationWarning
    >
      <body className="min-h-screen bg-cream font-sans text-ink">
        <Layout>{children}</Layout>
      </body>
    </html>
  );
}
