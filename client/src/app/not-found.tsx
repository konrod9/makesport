import Link from "next/link";
import { Compass } from "lucide-react";

export default function NotFound() {
  return (
    <div className="mx-auto flex min-h-[60vh] max-w-lg flex-col items-center justify-center px-5 text-center">
      <p className="font-display text-7xl font-extrabold text-lime-500">404</p>
      <h1 className="mt-4 font-display text-2xl font-bold">
        Такой площадки пока нет
      </h1>
      <p className="mt-3 text-ink-500">
        Возможно, её ещё не добавили. Загляните на карту или предложите новое
        место.
      </p>
      <div className="mt-7 flex gap-3">
        <Link
          href="/map"
          className="inline-flex items-center gap-2 rounded-full bg-ink px-6 py-3 text-sm font-semibold text-cream hover:bg-ink-700"
        >
          <Compass className="size-4" /> На карту
        </Link>
        <Link
          href="/add"
          className="rounded-full border border-ink-200 bg-white px-6 py-3 text-sm font-semibold hover:border-ink"
        >
          Добавить площадку
        </Link>
      </div>
    </div>
  );
}
