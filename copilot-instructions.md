# Sports Web Backend - Instrucciones para Desarrollo

## Stack & Estructura

- **Framework**: .NET 10 (C#)
- **ORM**: Entity Framework Core
- **Patterns**:
  - Controllers en `Controllers/`
  - Services en `Services/`
  - Models (Entities) en `Models/`
  - DTOs en `DTOs/`
  - Commands en `Features/[Entity]/Commands/`

## Convenciones Clave

1. **Naming**: PascalCase para DTOs y payloads
2. **Validation**: Usar FluentValidation para requests
3. **Error Handling**: GlobalExceptionHandler para respuestas consistentes
4. **Borrado Lógico**: DELETE marca `isActive = false`, no elimina

## Autenticación y Autorización

### Roles Disponibles

Aplicación soporta **5 roles escalables** con permisos jerárquicos:

```csharp
public static class UserRoles
{
    public const string Admin = nameof(Admin);                    // Control total
    public const string LeagueManager = nameof(LeagueManager);    // Gestiona ligas
    public const string TeamManager = nameof(TeamManager);        // Gestiona equipos
    public const string Player = nameof(Player);                  // Jugador registrado
    public const string Spectator = nameof(Spectator);            // Público/aficionado
}
```

### Estructura de User Entity

```csharp
// Models/User.cs
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }

    public List<string> Roles { get; set; } = new(); // Múltiples roles permitidos
    public Guid? TeamId { get; set; }                 // Para Player o TeamManager
    public Guid? LeagueId { get; set; }               // Para LeagueManager

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
```

### Patrones de Autorización en Controllers

```csharp
// Solo Admin
[Authorize(Roles = UserRoles.Admin)]
[HttpPost("leagues")]
public async Task<IActionResult> CreateLeague(CreateLeagueRequest request) { }

// Admin o LeagueManager
[Authorize(Roles = $"{UserRoles.Admin},{UserRoles.LeagueManager}")]
[HttpPost("leagues/{leagueId}/schedule")]
public async Task<IActionResult> GenerateSchedule(Guid leagueId) { }

// Admin o TeamManager (validar pertenencia a equipo)
[Authorize(Roles = $"{UserRoles.Admin},{UserRoles.TeamManager}")]
[HttpPut("matches/{matchId}")]
public async Task<IActionResult> UpdateMatchDateTime(Guid matchId) { }

// Público (sin [Authorize])
[HttpGet("matches/{matchId}/result")]
public async Task<IActionResult> GetMatchResult(Guid matchId) { }

// Cualquier usuario autenticado
[Authorize]
[HttpGet("statistics")]
public async Task<IActionResult> GetMyStatistics() { }
```

### Claims en JWT

El token JWT incluye automáticamente:
- `sub` - UserId
- `email` - Email del usuario
- `role` - Roles (múltiples claims si es necesario)
- `TeamId` - Si aplica (custom claim)
- `LeagueId` - Si aplica (custom claim)

### Endpoints de Autenticación

- ✅ `POST /api/auth/login` - Login y obtener token JWT
- ✅ `POST /api/auth/register` - Registro (por defecto rol "Player")
- ✅ `POST /api/auth/refresh` - Refresh token (si aplica)

## Checklist: Agregar Nuevo CRUD (Entity)

### 1. Models (Entities)

- [ ] Crear Entity en `Models/[Entity].cs`
  - Propiedades: `Id`, `CreatedAt`, `UpdatedAt`, `IsActive`
  - Validaciones básicas en atributos

### 2. DTOs

- [ ] `[Entity]Dto.cs` - para respuestas GET
- [ ] `Create[Entity]Request.cs` - payload POST (PascalCase)
- [ ] `Update[Entity]Request.cs` - payload PUT

### 3. Database

- [ ] DbSet en `DbContext`
- [ ] Migration: `dotnet ef migrations add Add[Entity]`

### 4. Services

- [ ] `I[Entity]Service.cs` - interface
- [ ] `[Entity]Service.cs` - implementación
  - `GetAll[Entities]()`
  - `Get[Entity]ById(id)`
  - `Create[Entity](request)`
  - `Update[Entity](id, request)` - retorna bool
  - `Delete[Entity](id)` - borrado lógico

### 5. Controllers

- [ ] `[Entities]Controller.cs`
  - `[HttpGet]` GetAll
  - `[HttpGet("{id}")]` GetById
  - `[HttpPost]` Create
  - `[HttpPut("{id}")]` Update
  - `[HttpDelete("{id}")]` Delete (lógico)
  - **Agregar autorización apropiada** (ver sección de Autenticación y Autorización)

### 6. Validación

- [ ] Crear validador con FluentValidation
- [ ] Registrar en DI container

### 7. Upload de Archivos

- [ ] Endpoint POST `/api/files/upload?folder=[name]`
  - Retornar: `{ success: true, fileUrl: string }`
  - Guardar en carpeta específica
  - Validar tipos MIME

### 8. Unit Tests (OBLIGATORIO)

- [ ] Siempre crear o actualizar tests para cada nueva implementación o modificación de un feature.
  - Proyecto de tests (usar existente): `Sports.API.Tests/Sports.API.Tests.csproj` (ruta relativa en la solución)
  - Estructura de carpetas sugerida dentro del proyecto de tests: `UnitTests/Services`, `UnitTests/Repositories`, `IntegrationTests/`, etc.
  - Frameworks recomendados: `xUnit`, `Moq`/`NSubstitute`, `FluentAssertions`
  - Convenciones de nombrado: `[Entity]ServiceTests.cs`, `[Entities]ControllerTests.cs`
  - Tests mínimos por CRUD: GetAll, GetById, Create, Update, Delete (happy path + validaciones + caminos de error)
  - Para cambios existentes: actualizar tests que cubran el comportamiento modificado y añadir pruebas de regresión si aplica
  - Añadir los tests a la solución y asegurar que `dotnet test` pase en CI

## Endpoints Requeridos por Frontend

Cuando crees una entidad, **DEBE tener estos 5 endpoints**:

- ✅ `GET /api/[entities]` - todos
- ✅ `GET /api/[entities]/{id}` - por ID (para editar)
- ✅ `POST /api/[entities]` - crear
- ✅ `PUT /api/[entities]/{id}` - actualizar (para editar)
- ✅ `DELETE /api/[entities]/{id}` - eliminar lógico (para borrar)
- ✅ `POST /api/files/upload` - subir archivos (si la entidad tiene fotos)

**Si falta alguno, el frontend no puede completar esa operación.**

## Estado Actual

- ✅ Teams: todos los endpoints completos
- ⚠️ Players: GetAll ✅, Create ✅, pero falta GetById, Update, Delete
- ⚠️ Matches: TBD
- ⚠️ Auth: Pendiente de implementación

## Notas Importantes

- **DTO en PascalCase**: el frontend espera `FirstName`, no `firstName`
- **Enums como strings**: enviar nombres literales ("Midfielder"), no números
- **Borrado lógico**: UPDATE `IsActive = false`, no DELETE
- **URLs relativas**: filePath debe ser relativa a la carpeta de upload
- **Validación en backend**: siempre validar los datos recibidos antes de procesarlos
- **Manejo de errores**: retornar mensajes claros y códigos HTTP adecuados (400, 404, 500, etc.)
- **Autorización**: Verificar permisos de usuario según rol y contexto (ej: TeamManager solo puede editar su equipo)
- **Password**: Siempre hashear con algoritmo seguro (BCrypt o PBKDF2), nunca en texto plano
- **Secretos**: JWT key en `appsettings.json` o variables de entorno, nunca hardcodeado
- **Secretos**: JWT key en `appsettings.json` o variables de entorno, nunca hardcodeado