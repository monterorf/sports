"# sports" 

````````

# Sports Management API

API RESTful para la gestión de deportes, equipos, jugadores y partidos construida con .NET 10, implementando Clean Architecture y CQRS.

## ??? Arquitectura

El proyecto sigue los principios de Clean Architecture con las siguientes capas:

- **Sports.Domain**: Entidades, enums e interfaces del dominio
- **Sports.Application**: Lógica de negocio, CQRS (Commands/Queries), DTOs y validaciones
- **Sports.Infrastructure**: Implementación de persistencia con Entity Framework Core
- **Sports.API**: API REST con ASP.NET Core

## ??? Tecnologías

- .NET 10
- Entity Framework Core 10
- MediatR (CQRS)
- AutoMapper
- FluentValidation
- SQL Server (LocalDB)
- Swagger/OpenAPI

## ?? Requisitos Previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server LocalDB (incluido con Visual Studio) o SQL Server

## ?? Configuración e Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/monterorf/sports
cd Sports
```

### 2. Restaurar paquetes NuGet

```bash
dotnet restore
```

### 3. Configurar la cadena de conexión

Edita `src/Sports.API/appsettings.json` si necesitas cambiar la cadena de conexión:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SportsDb;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

### 4. Aplicar migraciones y crear la base de datos

```bash
dotnet ef database update --project src/Sports.Infrastructure/Sports.Infrastructure.csproj --startup-project src/Sports.API/Sports.API.csproj
```

### 5. Ejecutar la aplicación

```bash
cd src/Sports.API
dotnet run
```

La API estará disponible en:
- HTTP: http://localhost:5199
- HTTPS: https://localhost:7173

## ?? Documentación de la API

Una vez ejecutada la aplicación, puedes acceder a la documentación Swagger en:

- https://localhost:7173/swagger
- http://localhost:5199/swagger

## ?? Endpoints Disponibles

### Teams (Equipos)

- `GET /api/teams` - Obtener todos los equipos
- `GET /api/teams/{id}` - Obtener un equipo por ID
- `POST /api/teams` - Crear un nuevo equipo

### Players (Jugadores)

- `GET /api/players` - Obtener todos los jugadores
- `POST /api/players` - Crear un nuevo jugador

### Matches (Partidos)

- `GET /api/matches` - Obtener todos los partidos

## ?? Estructura del Proyecto

```
Sports/
??? src/
?   ??? Sports.API/              # Capa de presentación (API REST)
?   ??? Sports.Application/      # Capa de aplicación (CQRS)
?   ??? Sports.Domain/           # Capa de dominio
?   ??? Sports.Infrastructure/   # Capa de infraestructura
??? tests/                       # Proyectos de pruebas
```

## ?? Patrones de Diseño Implementados

- **Clean Architecture**: Separación de responsabilidades en capas
- **CQRS**: Separación de comandos y consultas con MediatR
- **Repository Pattern**: Abstracción del acceso a datos
- **Unit of Work**: Gestión de transacciones
- **Dependency Injection**: Inversión de control
- **Domain-Driven Design**: Entidades ricas con lógica de negocio

## ?? Comandos Útiles de EF Core

### Crear una nueva migración
```bash
dotnet ef migrations add <MigrationName> --project src/Sports.Infrastructure/Sports.Infrastructure.csproj --startup-project src/Sports.API/Sports.API.csproj
```

### Aplicar migraciones
```bash
dotnet ef database update --project src/Sports.Infrastructure/Sports.Infrastructure.csproj --startup-project src/Sports.API/Sports.API.csproj
```

## ?? Autor

- GitHub: [@monterorf](https://github.com/monterorf)
