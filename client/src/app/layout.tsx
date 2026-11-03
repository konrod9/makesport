import type { Metadata } from "next";
import {
  Geist,
  Geist_Mono,
  Figtree,
  Manrope,
  Unbounded,
} from "next/font/google";
import "./globals.css";
import { cn } from "@/shared/lib/utils";
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

// const figtree = Figtree({ subsets: ["latin"], variable: "--font-sans" });

// const geistSans = Geist({
//   variable: "--font-geist-sans",
//   subsets: ["latin"],
// });

// const geistMono = Geist_Mono({
//   variable: "--font-geist-mono",
//   subsets: ["latin"],
// });

// export const metadata: Metadata = {
//   title: "MakeSport — поиск спортивных площадок",
//   description:
//     "Платформа для поиска и добавления спортивных площадок. Находите ближайшие площадки для различных видов спорта на карте.",
// };

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
