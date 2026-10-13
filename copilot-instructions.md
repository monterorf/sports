# Sports Web Backend - Instrucciones para Desarrollo

## Stack & Estructura

- **Framework**: .NET 8+ (C#)
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

### 6. Validación

- [ ] Crear validador con FluentValidation
- [ ] Registrar en DI container

### 7. Upload de Archivos

- [ ] Endpoint POST `/api/files/upload?folder=[name]`
  - Retornar: `{ success: true, fileUrl: string }`
  - Guardar en carpeta específica
  - Validar tipos MIME

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

## Notas Importantes

- **DTO en PascalCase**: el frontend espera `FirstName`, no `firstName`
- **Enums como strings**: enviar nombres literales ("Midfielder"), no números
- **Borrado lógico**: UPDATE `IsActive = false`, no DELETE
- **URLs relativas**: filePath debe ser relativa a la carpeta de upload