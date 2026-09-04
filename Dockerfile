# --- Etapa 1: build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos solo los .csproj primero para aprovechar la cache de capas de Docker
COPY ProductsApi.sln .
COPY src/Products.API/Products.API.csproj src/Products.API/
COPY src/Products.Application/Products.Application.csproj src/Products.Application/
COPY src/Products.Domain/Products.Domain.csproj src/Products.Domain/
COPY src/Products.Infrastructure/Products.Infrastructure.csproj src/Products.Infrastructure/
COPY tests/Products.Tests/Products.Tests.csproj tests/Products.Tests/

RUN dotnet restore ProductsApi.sln

# Ahora sí copiamos el resto del código y compilamos
COPY . .
RUN dotnet publish src/Products.API/Products.API.csproj -c Release -o /app/publish --no-restore

# --- Etapa 2: runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Products.API.dll"]