# Architecture Rules & Constraints

## Overview

This project follows **Clean Architecture** with four layers. Dependencies flow inward — outer layers depend on inner layers, never the reverse.

```
Sports.API  →  Sports.Application  →  Sports.Domain
    ↓                  ↓
Sports.Infrastructure  →  Sports.Domain
```

---

## Layer Responsibilities

### Sports.Domain
- Core business entities and enums
- Domain interfaces (repository contracts)
- No dependencies on other layers
- No framework dependencies (no EF, no ASP.NET)

### Sports.Application
- CQRS handlers (commands and queries)
- DTOs and AutoMapper profiles
- FluentValidation validators
- Dependency injection extension: `AddApplication()`
- Depends only on `Sports.Domain`

### Sports.Infrastructure
- Entity Framework Core implementations
- Repository and Unit of Work implementations
- EF entity configurations
- Database migrations
- Dependency injection extension: `AddInfrastructure(IConfiguration)`
- Depends on `Sports.Domain` and `Sports.Application`

### Sports.API
- ASP.NET Core controllers
- `Program.cs` entry point and middleware pipeline
- OpenAPI / Scalar documentation (Development only)
- Depends on `Sports.Application` and `Sports.Infrastructure`

---

## Hard Constraints

- **Domain must not reference any other project or framework** (EF, MediatR, etc.).
- **Application must not reference Infrastructure** — use interfaces defined in Domain.
- **Controllers must not contain business logic** — dispatch to MediatR only.
- **Repositories must not call `SaveChangesAsync()`** — that is the sole responsibility of `IUnitOfWork`.
- **Direct `DbContext` usage is forbidden outside of Infrastructure.**

---

## Dependency Injection Rules

- Each layer registers its own services via a static extension method:
  ```csharp
  // Sports.Application
  services.AddApplication();

  // Sports.Infrastructure
  services.AddInfrastructure(configuration);
  ```
- `Program.cs` must remain minimal — only compose layers and configure middleware.
- All services are registered with **Scoped** lifetime unless there is a specific reason otherwise.

---

## Startup Behavior

- Database migrations are applied automatically on startup:
  ```csharp
  context.Database.Migrate();
  ```
- OpenAPI documentation is exposed only in `Development` environment.
- HTTPS redirection is always enabled.
