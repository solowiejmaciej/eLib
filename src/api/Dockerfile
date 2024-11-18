FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_ENVIRONMENT=Docker

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY src/api/eLib.csproj api/
COPY src/eLib.Auth/eLib.Auth.csproj eLib.Auth/
COPY src/eLib.Events/eLib.Events.csproj eLib.Events/
COPY src/eLib.Common/eLib.Common.csproj eLib.Common/

RUN dotnet restore api/eLib.csproj

COPY src/ .

WORKDIR /src/api
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eLib.dll"]
