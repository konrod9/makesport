"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { Header } from "@/shared/components/header";
import { Input } from "@/shared/components/ui/input";
import { Label } from "@/shared/components/ui/label";
import { Checkbox } from "@/shared/components/ui/checkbox";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card";
import {
  Upload,
  X,
  MapPin,
  Clock,
  ImagePlus,
  ArrowLeft,
  Check,
} from "lucide-react";
import { cities, sportTypes, surfaces } from "@/shared/lib/data";
import Link from "next/link";
import { Textarea } from "@/shared/components/ui/textarea";
import { Button } from "@/shared/components/ui/button";

export default function AddVenuePage() {
  const router = useRouter();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [images, setImages] = useState<string[]>([]);

  const [formData, setFormData] = useState({
    name: "",
    description: "",
    city: "",
    address: "",
    sportType: "",
    surface: "",
    workingHoursStart: "08:00",
    workingHoursEnd: "22:00",
    hasLighting: false,
    isFree: true,
  });

  const handleImageUpload = () => {
    // Simulate image upload with placeholder
    const placeholders = [
      "https://images.unsplash.com/photo-1546519638-68e109498ffc?w=400",
      "https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400",
      "https://images.unsplash.com/photo-1551958219-acbc608c6377?w=400",
    ];
    if (images.length < 5) {
      setImages([...images, placeholders[images.length % placeholders.length]]);
    }
  };

  const removeImage = (index: number) => {
    setImages(images.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);

    // Simulate API call
    await new Promise((resolve) => setTimeout(resolve, 1500));

    setIsSubmitting(false);
    setIsSuccess(true);

    // Redirect after showing success
    setTimeout(() => {
      router.push("/");
    }, 2000);
  };

  if (isSuccess) {
    return (
      <div className="min-h-screen">
        <Header />
        <main className="mx-auto max-w-2xl px-4 sm:px-6 lg:px-8 py-16">
          <div className="flex flex-col items-center justify-center text-center">
            <div className="w-20 h-20 rounded-full bg-green-600/20 flex items-center justify-center mb-6">
              <Check className="h-10 w-10 text-green-500" />
            </div>
            <h1 className="text-2xl font-bold text-foreground mb-2">
              Площадка добавлена!
            </h1>
            <p className="text-muted-foreground mb-6">
              Спасибо за вклад в сообщество. Ваша площадка будет проверена
              модератором.
            </p>
            <Button asChild>
              <Link href="/">Вернуться к списку</Link>
            </Button>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="min-h-screen">
      <main className="mx-auto max-w-3xl px-4 sm:px-6 lg:px-8 py-8">
        <Link
          href="/"
          className="inline-flex items-center gap-2 text-muted-foreground hover:text-foreground transition-colors mb-6"
        >
          <ArrowLeft className="h-4 w-4" />
          Назад к площадкам
        </Link>

        <div className="mb-8">
          <h1 className="text-3xl font-bold text-foreground mb-2">
            Добавить <span className="text-chart-3">площадку</span>
          </h1>
          <p className="text-muted-foreground">
            Поделитесь информацией о спортивной площадке с другими
            пользователями
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Basic Info */}
          <Card className="border-border bg-card">
            <CardHeader>
              <CardTitle className="text-lg">Основная информация</CardTitle>
              <CardDescription>Название и описание площадки</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="name">Название площадки *</Label>
                <Input
                  id="name"
                  placeholder="Например: Баскетбольная площадка в парке Горького"
                  value={formData.name}
                  onChange={(e) =>
                    setFormData({ ...formData, name: e.target.value })
                  }
                  className="bg-input border-border"
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="description">Описание</Label>
                <Textarea
                  id="description"
                  placeholder="Опишите площадку: состояние, особенности, что есть рядом..."
                  value={formData.description}
                  onChange={(e) =>
                    setFormData({ ...formData, description: e.target.value })
                  }
                  className="bg-input border-border min-h-[120px] resize-none"
                />
              </div>
            </CardContent>
          </Card>

          {/* Location */}
          <Card className="border-border bg-card">
            <CardHeader>
              <CardTitle className="text-lg flex items-center gap-2">
                <MapPin className="h-5 w-5 text-chart-3" />
                Местоположение
              </CardTitle>
              <CardDescription>Где находится площадка</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="city">Город *</Label>
                  <Select
                    value={formData.city}
                    onValueChange={(value) =>
                      setFormData({ ...formData, city: value })
                    }
                  >
                    <SelectTrigger id="city" className="bg-input border-border">
                      <SelectValue placeholder="Выберите город" />
                    </SelectTrigger>
                    <SelectContent>
                      {cities.map((city) => (
                        <SelectItem key={city} value={city}>
                          {city}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div className="space-y-2">
                  <Label htmlFor="address">Адрес *</Label>
                  <Input
                    id="address"
                    placeholder="ул. Примерная, 123"
                    value={formData.address}
                    onChange={(e) =>
                      setFormData({ ...formData, address: e.target.value })
                    }
                    className="bg-input border-border"
                    required
                  />
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Sports Info */}
          <Card className="border-border bg-card">
            <CardHeader>
              <CardTitle className="text-lg">Характеристики</CardTitle>
              <CardDescription>Тип площадки и покрытие</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="sportType">Вид спорта *</Label>
                  <Select
                    value={formData.sportType}
                    onValueChange={(value) =>
                      setFormData({ ...formData, sportType: value })
                    }
                  >
                    <SelectTrigger
                      id="sportType"
                      className="bg-input border-border"
                    >
                      <SelectValue placeholder="Выберите вид спорта" />
                    </SelectTrigger>
                    <SelectContent>
                      {sportTypes.map((sport) => (
                        <SelectItem key={sport} value={sport}>
                          {sport}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div className="space-y-2">
                  <Label htmlFor="surface">Покрытие *</Label>
                  <Select
                    value={formData.surface}
                    onValueChange={(value) =>
                      setFormData({ ...formData, surface: value })
                    }
                  >
                    <SelectTrigger
                      id="surface"
                      className="bg-input border-border"
                    >
                      <SelectValue placeholder="Выберите покрытие" />
                    </SelectTrigger>
                    <SelectContent>
                      {surfaces.map((surface) => (
                        <SelectItem key={surface} value={surface}>
                          {surface}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Working Hours & Features */}
          <Card className="border-border bg-card">
            <CardHeader>
              <CardTitle className="text-lg flex items-center gap-2">
                <Clock className="h-5 w-5 text-chart-3" />
                Время работы и услуги
              </CardTitle>
              <CardDescription>
                Часы работы и доступные удобства
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="workingHoursStart">Открытие</Label>
                  <Input
                    id="workingHoursStart"
                    type="time"
                    value={formData.workingHoursStart}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        workingHoursStart: e.target.value,
                      })
                    }
                    className="bg-input border-border"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="workingHoursEnd">Закрытие</Label>
                  <Input
                    id="workingHoursEnd"
                    type="time"
                    value={formData.workingHoursEnd}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        workingHoursEnd: e.target.value,
                      })
                    }
                    className="bg-input border-border"
                  />
                </div>
              </div>
              <div className="flex flex-col sm:flex-row gap-4 pt-2">
                <div className="flex items-center space-x-2">
                  <Checkbox
                    id="hasLighting"
                    checked={formData.hasLighting}
                    onCheckedChange={(checked) =>
                      setFormData({
                        ...formData,
                        hasLighting: checked as boolean,
                      })
                    }
                  />
                  <Label htmlFor="hasLighting" className="cursor-pointer">
                    Есть освещение
                  </Label>
                </div>
                <div className="flex items-center space-x-2">
                  <Checkbox
                    id="isFree"
                    checked={formData.isFree}
                    onCheckedChange={(checked) =>
                      setFormData({ ...formData, isFree: checked as boolean })
                    }
                  />
                  <Label htmlFor="isFree" className="cursor-pointer">
                    Бесплатная
                  </Label>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Images */}
          <Card className="border-border bg-card">
            <CardHeader>
              <CardTitle className="text-lg flex items-center gap-2">
                <ImagePlus className="h-5 w-5 text-chart-3" />
                Фотографии
              </CardTitle>
              <CardDescription>
                Добавьте до 5 фотографий площадки
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-5 gap-3">
                {images.map((image, index) => (
                  <div
                    key={index}
                    className="relative aspect-square rounded-lg overflow-hidden group"
                  >
                    <img
                      src={image}
                      alt={`Фото ${index + 1}`}
                      className="w-full h-full object-cover"
                    />
                    <button
                      type="button"
                      onClick={() => removeImage(index)}
                      className="absolute top-1 right-1 p-1 rounded-full bg-background/80 text-foreground opacity-0 group-hover:opacity-100 transition-opacity"
                    >
                      <X className="h-4 w-4" />
                    </button>
                  </div>
                ))}
                {images.length < 5 && (
                  <button
                    type="button"
                    onClick={handleImageUpload}
                    className="aspect-square rounded-lg border-2 border-dashed border-border hover:border-chart-3/50 flex flex-col items-center justify-center gap-2 text-muted-foreground hover:text-foreground transition-colors"
                  >
                    <Upload className="h-6 w-6" />
                    <span className="text-xs">Загрузить</span>
                  </button>
                )}
              </div>
            </CardContent>
          </Card>

          {/* Submit */}
          <div className="flex flex-col sm:flex-row gap-3 pt-4">
            <Button
              type="submit"
              size="lg"
              className="flex-1"
              disabled={
                isSubmitting ||
                !formData.name ||
                !formData.city ||
                !formData.address ||
                !formData.sportType ||
                !formData.surface
              }
            >
              {isSubmitting ? "Отправка..." : "Добавить площадку"}
            </Button>
            <Button
              type="button"
              variant="outline"
              size="lg"
              onClick={() => router.push("/")}
            >
              Отмена
            </Button>
          </div>
        </form>
      </main>
    </div>
  );
}
