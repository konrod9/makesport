FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID

WORKDIR /app
EXPOSE 8002

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG NUGET_USERNAME
ARG NUGET_PASSWORD
WORKDIR /src

COPY ["nuget.config", "."]
    
RUN dotnet nuget update source gitlab --username $NUGET_USERNAME --password $NUGET_PASSWORD --store-password-in-clear-text \
    --configfile nuget.config

COPY ["FileService/", "FileService/"]

RUN dotnet restore "FileService/src/FileService.API/FileService.API.csproj"

RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="/root/.dotnet/tools:${PATH}"

WORKDIR "/src/FileService/src/FileService.API"

RUN dotnet ef migrations bundle \
    --project "../VenuesService.Infrastructure.Postgres/VenuesService.Infrastructure.Postgres.csproj" \
    --startup-project "VenuesService.API.csproj" \
    --output /app/efbundle \
    --configuration Release \
    --verbose \
    -- --configfile /src/nuget.config

RUN chmod +x /app/efbundle

RUN dotnet build "FileService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FileService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /app/efbundle .
ENTRYPOINT ["dotnet", "FileService.API.dll"]
