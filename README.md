# Products API — Prueba Técnica Backend (Fundación delamujer)
 
API REST para la gestión de un catálogo de productos y su stock, construida en **.NET 8 (C#)** aplicando **Clean Architecture**, pensada para ser el núcleo transaccional consumido por múltiples plataformas concurrentes.
 
## Tabla de contenido
 
- [Arquitectura](#arquitectura)
- [Reglas de negocio](#reglas-de-negocio)
- [Endpoints](#endpoints)
- [Cómo correr el proyecto](#cómo-correr-el-proyecto)
  - [Opción A: local con .NET](#opción-a-local-con-net)
  - [Opción B: Docker](#opción-b-docker)
- [Migraciones de base de datos](#migraciones-de-base-de-datos)
- [Pruebas](#pruebas)
- [Despliegue](#despliegue)
- [Decisiones técnicas](#decisiones-técnicas)
## Arquitectura
 
El proyecto está organizado en 4 capas (Clean Architecture), con las dependencias apuntando siempre hacia el dominio:
 
```
products-api/
├── src/
│   ├── Products.API/            → Presentación: controllers, Swagger, middleware, Program.cs
│   ├── Products.Application/    → Casos de uso: DTOs, validaciones, ProductService
│   ├── Products.Domain/         → Núcleo: entidad Product, reglas de negocio, contratos (interfaces)
│   └── Products.Infrastructure/ → EF Core + Npgsql, repositorios, migraciones
├── tests/
│   └── Products.Tests/          → Pruebas unitarias de las reglas de negocio
├── Dockerfile
├── docker-compose.yml
└── ProductsApi.sln
```
 
`Products.Domain` no depende de ningún otro proyecto ni de EF Core: las reglas de negocio (por ejemplo, que el stock no pueda quedar negativo) viven en la entidad `Product` y se pueden probar sin necesidad de base de datos, como se ve en `tests/Products.Tests/ProductTests.cs`.
 
## Reglas de negocio
 
**Stock nunca negativo:** la entidad `Product` no expone un setter público de `Stock`. La única forma de modificarlo es el método `AdjustStock(int delta)`, que valida que `Stock + delta >= 0` antes de aplicar el cambio; si no se cumple, lanza `InsufficientStockException` (→ HTTP 400) y el stock **no** se modifica.
 
**Contrato explícito para evitar ambigüedad de signos:** en vez de recibir un número que puede ser positivo o negativo (fácil de mal-interpretar en el request), el endpoint de stock recibe:
 
```json
{
  "operationType": "Increase",  // o "Decrease"
  "quantity": 5                 // siempre positivo
}
```
 
`StockOperationType` es un enum (`Increase = 1`, `Decrease = 2`). El `ProductService` traduce esto a un delta (`+quantity` o `-quantity`) antes de llamar a `AdjustStock`. Esto hace el contrato de la API más legible en Swagger y evita que un cliente mande `-5` por error pensando que es positivo.
 
**Validaciones de entrada:** con `DataAnnotations` sobre los DTOs (`[Required]`, `[Range]`, `[StringLength]`), que ASP.NET Core valida automáticamente antes de llegar al controller, devolviendo 400 con el detalle del campo inválido.
 
**Concurrencia:** la tabla `products` usa la columna de sistema `xmin` de Postgres como token de concurrencia optimista (configurado en `ProductConfiguration`). Si dos requests intentan modificar el mismo producto al mismo tiempo, EF Core detecta el conflicto en el segundo `SaveChanges` y lanza `DbUpdateConcurrencyException`, evitando que un ajuste de stock pise silenciosamente al otro — relevante porque el enunciado menciona plataformas concurrentes.
 
## Endpoints
 
| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| POST | `/api/products` | Crea un producto | 201, 400 |
| GET | `/api/products/{id}` | Consulta un producto por id | 200, 404 |
| GET | `/api/products?pageNumber=&pageSize=` | Lista paginada de productos | 200 |
| PUT | `/api/products/{id}` | Actualiza nombre/descripción/precio | 200, 400, 404 |
| PATCH | `/api/products/{id}/stock` | Suma o resta unidades al stock | 200, 400, 404 |
| DELETE | `/api/products/{id}` | Elimina un producto | 204, 404 |
| GET | `/health` | Health check | 200 |
 
Documentación interactiva completa en `/swagger` una vez levantado el proyecto.
 
Los errores de negocio se devuelven en formato estándar `ProblemDetails` (RFC 7807), por ejemplo:
 
```json
{
  "status": 400,
  "title": "Stock insuficiente",
  "detail": "Stock insuficiente para el producto ... Stock actual: 5, ajuste solicitado: -10.",
  "instance": "/api/products/{id}/stock"
}
```
 
## Cómo correr el proyecto
 
### Opción A: local con .NET
 
Requisitos: .NET 8 SDK, acceso a una instancia de PostgreSQL.
 
1. Configura la cadena de conexión en `src/Products.API/appsettings.Development.json` (o mejor, en variables de entorno / `dotnet user-secrets` si vas a usar credenciales reales):
```bash
   dotnet user-secrets init --project src/Products.API
   dotnet user-secrets set "ConnectionStrings:ProductsDb" "Host=...;Port=5432;Database=...;Username=...;Password=..." --project src/Products.API
```
2. Aplica las migraciones:
```bash
   dotnet ef database update --project src/Products.Infrastructure --startup-project src/Products.API
```
3. Levanta la API:
```bash
   dotnet run --project src/Products.API
```
4. Abre `https://localhost:5081/swagger`.
### Opción B: Docker
 
```bash
docker compose up --build
```
 
Esto levanta un contenedor de Postgres 16 + la API (`http://localhost:8080/swagger`). Para apuntar a un servidor Postgres remoto en vez del contenedor local, sobrescribe la variable de entorno `ConnectionStrings__ProductsDb` del servicio `api` en `docker-compose.yml` (o en un archivo `.env`).
 
## Migraciones de base de datos
 
```bash
# Crear una nueva migración
dotnet ef migrations add NombreMigracion --project src/Products.Infrastructure --startup-project src/Products.API
 
# Aplicarla
dotnet ef database update --project src/Products.Infrastructure --startup-project src/Products.API
```
 
La API también aplica las migraciones pendientes automáticamente al iniciar (`Program.cs`, `dbContext.Database.Migrate()`), pensado para simplificar el despliegue de esta prueba técnica. En un sistema productivo real esto normalmente sería un paso explícito del pipeline de CI/CD.
 
## Pruebas
 
```bash
dotnet test
```
 
Cubren las invariantes de negocio de `Product`: creación con datos inválidos, incremento/decremento de stock, e intento de dejar el stock en negativo.
 
## Despliegue
 
**URL pública:** `<COMPLETAR>`
**Swagger:** `<COMPLETAR>/swagger`
 
## Decisiones técnicas
 
- **Clean Architecture** sobre N-Capas simple para mantener el dominio (reglas de stock) aislado y testeable sin depender de EF Core ni de ASP.NET Core.
- **PostgreSQL** por ser relacional, gratuito, y con buen soporte nativo en EF Core vía Npgsql.
- **Mapeo manual** (`ProductMapper`) en vez de AutoMapper: con solo un DTO de respuesta no se justifica la dependencia adicional.
- **DataAnnotations** en vez de FluentValidation: las validaciones de este dominio son simples (requerido, rango, longitud), no amerita una librería extra.
- **`xmin` de Postgres** como token de concurrencia en vez de una columna `RowVersion` manual: aprovecha un mecanismo nativo del motor de base de datos.
