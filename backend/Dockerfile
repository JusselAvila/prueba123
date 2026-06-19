# ---------------------------------------------------------------------------
# AJUSTA ESTO antes de construir:
#   1. DOTNET_VERSION: corre "dotnet --version" o revisa tu .csproj
#      (<TargetFramework>net?.0</TargetFramework>) y pon ese número.
#   2. El nombre "EventosAPI.dll" en el ENTRYPOINT: si tu .csproj se llama
#      distinto (ej. EventosAPI.Backend.csproj), el .dll publicado también
#      se llamará distinto. Revisa /app/publish después del build si falla.
#   3. Este Dockerfile asume que vive en una carpeta que SOLO contiene tu
#      proyecto de backend (ej. backend/Dockerfile, junto a tu .csproj o tu
#      .sln). Si tu estructura es distinta, ajusta el "context" en
#      docker-compose.yml en vez de este archivo.
# ---------------------------------------------------------------------------
ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

# Copiamos solo los archivos de proyecto primero para aprovechar el cache de
# Docker: si solo cambia tu código (no las dependencias), "dotnet restore" no
# se vuelve a ejecutar en cada build.
COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5158
ENTRYPOINT ["dotnet", "EventosAPI.dll"]
