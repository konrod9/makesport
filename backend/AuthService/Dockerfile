FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID

WORKDIR /app
EXPOSE 8003

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG NUGET_USERNAME
ARG NUGET_PASSWORD
WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Shared/", "Shared/"]
    
RUN dotnet nuget update source gitlab --username $NUGET_USERNAME --password $NUGET_PASSWORD --store-password-in-clear-text \
    --configfile nuget.config

COPY ["AuthService/", "AuthService/"]

RUN dotnet restore "AuthService/src/AuthService.API/AuthService.API.csproj"

RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="/root/.dotnet/tools:${PATH}"

WORKDIR "/src/AuthService/src/AuthService.API"

RUN dotnet ef migrations bundle \
    --project "../AuthService.Infrastructure/AuthService.Infrastructure.csproj" \
    --startup-project "AuthService.API.csproj" \
    --output /app/efbundle \
    --configuration Release \
    --verbose \
    -- --configfile /src/nuget.config

RUN chmod +x /app/efbundle

RUN dotnet build "AuthService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
COPY --from=build /app/efbundle .

ENTRYPOINT ["dotnet", "AuthService.API.dll"]
