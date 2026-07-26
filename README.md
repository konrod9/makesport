# MakeSport

Платформа для поиска и добавления спортивных площадок. Позволяет пользователям находить ближайшие площадки для различных видов спорта, просматривать их на карте, фильтровать по параметрам и добавлять новые места.

## Возможности

- **Поиск площадок** — просмотр списка спортивных площадок с бесконечной прокруткой
- **Карта** — интерактивная карта (Яндекс.Карты) с отображением всех площадок
- **Фильтрация** — фильтрация по виду спорта, покрытию, городу и другим параметрам (фильтры сохраняются в localStorage)
- **Добавление площадок** — форма для добавления новой площадки с выбором местоположения на карте и загрузкой изображений
- **Загрузка файлов** — поддержка multipart-загрузки изображений с хранением в S3-совместимом хранилище (MinIO)

## Архитектура

```
┌─────────────┐     ┌──────────────┐     ┌──────────────┐
│   Client    │────▶│    Nginx     │────▶│ VenuesService│
│ (Next.js 16)│     │  (API gw)    │     │  .NET 10 API │
│   :3000     │     │    :80       │     │    :5163     │
└─────────────┘     │              │     └──────┬───────┘
                    │              │            │
                    │              │────▶┌──────┴───────┐
                    │              │     │ FileService  │
                    │              │     │  .NET 10 API │
                    │              │     │    :8002     │
                    └──────────────┘     └──────┬───────┘
                                                │
                                        ┌───────┴────────┐
                                        │    MinIO (S3)   │
                                        │   :9000/:9001   │
                                        └────────────────┘
```

### Компоненты

| Компонент | Технологии | Порт |
|-----------|-----------|------|
| **Client** | Next.js 16, React 19, Tailwind v4, shadcn/ui, TanStack Query v5, Zustand v5 | 3000 |
| **VenuesService** | .NET 10, EF Core + Npgsql/PostGIS, CQRS | 5163 |
| **FileService** | .NET 10, EF Core + Postgres, MinIO (S3), Redis | 8002 |
| **PostgreSQL** | PostGIS 17-3.5 | 5433 |
| **MinIO** | S3-совместимое объектное хранилище | 9000/9001 |
| **Redis** | Кэширование | 6379 |
| **Seq** | Централизованное логирование (Serilog) | 8081/5341 |
| **Nginx** | API Gateway (маршрутизация /api/venues → venues, /api/files → files) | 80 |

## Технологический стек

### Frontend

- **Next.js 16** (App Router, React Compiler, standalone output)
- **React 19**
- **Tailwind CSS v4**
- **shadcn/ui** (base-ui)
- **TanStack Query v5** — серверное состояние и кэширование
- **Zustand v5** — клиентское состояние (фильтры сохраняются в localStorage)
- **Axios** — HTTP-клиент с перехватом ошибок на основе Envelope-ответов
- **react-hook-form** — формы
- **Яндекс.Карты** (@pbe/react-yandex-maps)
- **TypeScript**

### Backend (VenuesService & FileService)

- **.NET 10** (net10.0, nullable enabled, implicit usings)
- **Clean Architecture** — API / Application / Domain / Infrastructure слои
- **EF Core** + Npgsql с PostGIS (NetTopologySuite)
- **CSharpFunctionalExtensions** — Result/Error паттерн
- **Serilog** + Seq — структурированное логирование
- **xUnit** + **Testcontainers** — интеграционные тесты
- **MinIO** (S3 API) — хранение файлов (FileService)

## Быстрый старт

### Предварительные требования

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (для инфраструктуры и интеграционных тестов)
- [Node.js 25+](https://nodejs.org/) (для клиента)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (для бэкенда)

> На Windows используйте PowerShell для всех команд.

### 1. Запуск инфраструктуры

```powershell
docker compose -f docker-compose-dev.yml up -d
```

Запускает PostgreSQL (PostGIS), MinIO, Redis, Seq и Nginx.

### 2. Настройка переменных окружения

Создайте файл `.env` в корне проекта на основе образца (необходим для NuGet-аутентификации):

```
NUGET_USERNAME=<your-gitlab-username>
NUGET_PASSWORD=<your-gitlab-token>
```

### 3. Запуск бэкенда

**VenuesService:**
```powershell
cd backend/VenuesService
dotnet run --project src/VenuesService.API/VenuesService.API.csproj --environment Development
```

**FileService:**
```powershell
cd backend/FileService
dotnet run --project src/FileService.API/FileService.API.csproj --environment Development
```

### 4. Запуск клиента

```powershell
cd client
npm ci
npm run dev
```

Приложение будет доступно по адресу `http://localhost:3000`.

## Структура проекта

```
makesport/
├── client/                          # Next.js frontend
│   ├── src/
│   │   ├── app/                     # App Router страницы
│   │   │   ├── page.tsx             # /
│   │   │   ├── add/page.tsx         # /add — добавление площадки
│   │   │   ├── map/page.tsx         # /map — карта
│   │   │   └── venues/page.tsx      # /venues — список площадок
│   │   ├── entities/                # Бизнес-сущности (venues, file)
│   │   ├── features/                # Фичи (layout, venues)
│   │   └── shared/                  # Общие компоненты и утилиты
│   │       ├── api/                 # Axios, query client
│   │       ├── components/          # UI и бизнес-компоненты
│   │       └── lib/                 # Утилиты, статические данные
│   ├── next.config.ts
│   └── Dockerfile
│
├── backend/
│   ├── nuget.config                 # Общий NuGet config (nuget.org + gitlab)
│   ├── VenuesService/
│   │   ├── VenuesService.sln
│   │   ├── src/
│   │   │   ├── VenuesService.API/           # API endpoints
│   │   │   ├── VenuesService.Application/   # Use cases, CQRS
│   │   │   ├── VenuesService.Contracts/     # DTOs, requests/responses
│   │   │   ├── VenuesService.Domain/        # Domain model
│   │   │   └── VenuesService.Infrastructure.Postgres/  # EF Core, repositories
│   │   └── tests/
│   │       └── VenuesService.IntegrationTests/
│   ├── FileService/
│   │   ├── FileService.sln
│   │   ├── src/
│   │   │   ├── FileService.API/
│   │   │   ├── FileService.Application/
│   │   │   ├── FileService.Contracts/       # Включает IFileCommunicationService
│   │   │   ├── FileService.Domain/
│   │   │   ├── FileService.Infrastructure.Postgres/
│   │   │   ├── FileService.Infrastructure.Redis/
│   │   │   └── FileService.Infrastructure.S3/  # MinIO провайдер
│   │   └── tests/
│   │       └── FileService.IntegrationTests/
│   └── Dockerfiles (в каждом сервисе)
│
├── docker-compose-dev.yml          # Инфраструктура для локальной разработки
├── nginx.conf                       # API Gateway конфигурация
└── .env                             # Переменные окружения (NuGet auth)
```

## Команды

### Клиент

```powershell
cd client
npm run dev      # Запуск в режиме разработки
npm run build    # Production сборка
npm run lint     # ESLint
npm start        # Production сервер (после build)
npx tsc --noEmit # Проверка типов
```

### Бэкенд

```powershell
# Сборка
dotnet build src/VenuesService.API/VenuesService.API.csproj
dotnet build src/FileService.API/FileService.API.csproj

# Тесты (требуется Docker)
dotnet test tests/VenuesService.IntegrationTests/VenuesService.IntegrationTests.csproj
dotnet test tests/FileService.IntegrationTests/FileService.IntegrationTests.csproj
```

### Миграции EF Core

```powershell
dotnet ef migrations add <Name> --project src/VenuesService.Infrastructure.Postgres --startup-project src/VenuesService.API
```

## Docker-деплой

### Сборка и запуск всех сервисов

```powershell
docker compose -f docker-compose-dev.yml up -d --build
```

Сборка бэкендов требует GitLab-токен для доступа к приватным NuGet-пакетам (`MakeSport.*`). Токен передаётся через build args (`NUGET_USERNAME`, `NUGET_PASSWORD`) из `.env`.

В compose-файле также закомментирован сервис `frontend` — раскомментируйте для запуска клиента в Docker.

## API

### Маршрутизация (Nginx)

| Публичный endpoint | Внутренний endpoint |
|---|---|
| `GET /api/venues/*` | `http://venues:5163/venues/*` |
| `POST /api/venues/*` | `http://venues:5163/venues/*` |
| `GET /api/files/*` | `http://file-service:8002/files/*` |
| `POST /api/files/*` | `http://file-service:8002/files/*` |

### Формат ответа

Все ответы API обёрнуты в `Envelope<T>`:

```json
{
  "isError": false,
  "data": { ... },
  "error": null
}
```

При ошибке: `isError = true`, `data = null`, `error` содержит детали.

## Тестирование

- **Фреймворк**: xUnit
- **Инфраструктура**: Testcontainers (поднимает PostgreSQL с PostGIS, MinIO)
- **Требования**: Docker Desktop (контейнеры создаются автоматически для каждого test-класса)
- **FileService**: тесты используют реальный MinIO через Testcontainers
- **VenuesService**: HTTP-взаимодействие с FileService мокируется (`IFileCommunicationService`)

```powershell
# Запуск всех тестов
dotnet test
```

## Переменные окружения

| Переменная | Описание |
|---|---|
| `NUGET_USERNAME` | Имя пользователя для GitLab NuGet registry |
| `NUGET_PASSWORD` | Токен для GitLab NuGet registry |
| `NEXT_PUBLIC_API_BASE_URL` | Базовый URL API для клиента (по умолч. `http://localhost/api`) |
| `NEXT_PUBLIC_YMAPS_API_KEY` | API-ключ Яндекс.Карт |
