# Architecture Compliance Checklist

Este documento proporciona un checklist exhaustivo para validar el cumplimiento de las reglas de arquitectura Clean Architecture definidas en `01-architecture.md`.

---

## 📋 Índice

1. [Estructura de Capas y Referencias de Proyectos](#1-estructura-de-capas-y-referencias-de-proyectos)
2. [Capa Domain](#2-capa-domain)
3. [Capa Application](#3-capa-application)
4. [Capa Infrastructure](#4-capa-infrastructure)
5. [Capa API](#5-capa-api)
6. [Dependency Injection](#6-dependency-injection)
7. [Patrón Repository](#7-patrón-repository)
8. [Unit of Work](#8-unit-of-work)
9. [CQRS con MediatR](#9-cqrs-con-mediatr)
10. [Validación con FluentValidation](#10-validación-con-fluentvalidation)
11. [Entity Framework Core](#11-entity-framework-core)
12. [Program.cs y Middleware Pipeline](#12-programcs-y-middleware-pipeline)

---

## 1. Estructura de Capas y Referencias de Proyectos

### ✅ Verificaciones de Referencias

#### 1.1 Sports.Domain NO debe referenciar ningún proyecto
```xml
<!-- ✅ CORRECTO: src\Sports.Domain\Sports.Domain.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  
  <!-- ❌ NO DEBE TENER: -->
  <!-- <ItemGroup>
    <ProjectReference Include="..." />
    <PackageReference Include="Microsoft.EntityFrameworkCore" />
    <PackageReference Include="MediatR" />
  </ItemGroup> -->
</Project>
```

**Regla:** Domain es el núcleo y NO depende de nadie.

---

#### 1.2 Sports.Application SOLO debe referenciar Sports.Domain
```xml
<!-- ✅ CORRECTO: src\Sports.Application\Sports.Application.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <!-- ✅ CORRECTO: Solo Domain -->
    <ProjectReference Include="..\Sports.Domain\Sports.Domain.csproj" />
  </ItemGroup>

  <ItemGroup>
    <!-- ✅ CORRECTO: Paquetes permitidos -->
    <PackageReference Include="MediatR" Version="12.4.1" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
    <PackageReference Include="FluentValidation" Version="11.11.0" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.11.0" />
  </ItemGroup>

  <!-- ❌ NO DEBE TENER: -->
  <!-- <ProjectReference Include="..\Sports.Infrastructure\..." /> -->
  <!-- <PackageReference Include="Microsoft.EntityFrameworkCore" /> -->
</Project>
```

**Regla:** Application depende SOLO de Domain y sus abstracciones.

---

#### 1.3 Sports.Infrastructure debe referenciar Domain y Application
```xml
<!-- ✅ CORRECTO: src\Sports.Infrastructure\Sports.Infrastructure.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <!-- ✅ Referencias requeridas -->
    <ProjectReference Include="..\Sports.Domain\Sports.Domain.csproj" />
    <ProjectReference Include="..\Sports.Application\Sports.Application.csproj" />
  </ItemGroup>

  <ItemGroup>
    <!-- ✅ EF Core y SQL Server -->
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0">
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

**Regla:** Infrastructure implementa las interfaces definidas en Domain y puede referenciar Application para configuraciones específicas.

---

#### 1.4 Sports.API debe referenciar Application e Infrastructure
```xml
<!-- ✅ CORRECTO: src\Sports.API\Sports.API.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <!-- ✅ Referencias requeridas -->
    <ProjectReference Include="..\Sports.Application\Sports.Application.csproj" />
    <ProjectReference Include="..\Sports.Infrastructure\Sports.Infrastructure.csproj" />
  </ItemGroup>

  <ItemGroup>
    <!-- ✅ Paquetes API -->
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.1" />
    <PackageReference Include="Scalar.AspNetCore" Version="1.2.61" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0">
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

  <!-- ❌ NO DEBE referenciar Sports.Domain directamente -->
  <!-- Acceso a Domain es a través de Application -->
</Project>
```

**Regla:** API orquesta las capas Application e Infrastructure, sin acceso directo a Domain.

---

## 2. Capa Domain

### ✅ 2.1 Entidades de Dominio

#### Estructura Correcta
```csharp
// ✅ CORRECTO: src\Sports.Domain\Entities\Team.cs
using Sports.Domain.Common;
using Sports.Domain.Enums;

namespace Sports.Domain.Entities;

public class Team : BaseEntity
{
    // ✅ Propiedades con private set (encapsulación)
    public string Name { get; private set; }
    public string ShortName { get; private set; }
    public SportType SportType { get; private set; }
    public bool IsActive { get; private set; }

    // ✅ Colecciones backing fields (encapsulación de colecciones)
    private readonly List<Player> _players = new();
    public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

    // ✅ Constructor privado para EF Core
    private Team() { }

    // ✅ Constructor público con validaciones
    public Team(
        string name,
        string shortName,
        SportType sportType)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ShortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
        SportType = sportType;
        IsActive = true;
    }

    // ✅ Métodos de comportamiento de dominio
    public void UpdateTeamInfo(string name, string shortName)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ShortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
        SetUpdatedAt();
    }

    public void AddPlayer(Player player)
    {
        if (player == null)
            throw new ArgumentNullException(nameof(player));

        if (_players.Any(p => p.Id == player.Id))
            throw new InvalidOperationException("Player already exists");

        _players.Add(player);
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
```

#### ❌ Anti-patrones a EVITAR
```csharp
// ❌ INCORRECTO: Propiedades públicas mutables
public class Team
{
    public string Name { get; set; }  // ❌ set público
    public List<Player> Players { get; set; }  // ❌ List expuesta
}

// ❌ INCORRECTO: Dependencia de EF Core
using Microsoft.EntityFrameworkCore;  // ❌ NO en Domain

public class Team
{
    [Required]  // ❌ Data Annotations de EF
    public string Name { get; set; }
}

// ❌ INCORRECTO: Referencias a capas superiores
using Sports.Infrastructure;  // ❌ Domain no conoce Infrastructure
using Sports.Application;     // ❌ Domain no conoce Application
```

**Checklist:**
- [ ] Todas las propiedades tienen `private set`
- [ ] Colecciones usan backing fields con `IReadOnlyCollection<T>`
- [ ] Constructor privado sin parámetros para EF Core
- [ ] Constructor público con validaciones
- [ ] Métodos de comportamiento encapsulan la lógica de negocio
- [ ] NO hay using de `Microsoft.EntityFrameworkCore`
- [ ] NO hay using de `MediatR`
- [ ] NO hay Data Annotations de EF (`[Required]`, `[MaxLength]`, etc.)

---

### ✅ 2.2 Interfaces de Repositorio

```csharp
// ✅ CORRECTO: src\Sports.Domain\Interfaces\ITeamRepository.cs
using Sports.Domain.Entities;

namespace Sports.Domain.Interfaces;

public interface ITeamRepository
{
    // ✅ Métodos async con CancellationToken
    Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Team>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Team?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Team> AddAsync(Team team, CancellationToken cancellationToken = default);
    Task UpdateAsync(Team team, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
```

#### ❌ Anti-patrones
```csharp
// ❌ INCORRECTO: SaveChanges en repositorio
public interface ITeamRepository
{
    Task AddAsync(Team team);
    Task SaveChangesAsync();  // ❌ Esto es responsabilidad de UnitOfWork
}

// ❌ INCORRECTO: Exponer DbContext
public interface ITeamRepository
{
    DbContext Context { get; }  // ❌ DbContext no debe exponerse
}

// ❌ INCORRECTO: DTOs en interfaces de Domain
public interface ITeamRepository
{
    Task<TeamDto> GetByIdAsync(Guid id);  // ❌ DTOs son de Application
}
```

**Checklist:**
- [ ] Interfaces definen contratos de persistencia
- [ ] Métodos async con `CancellationToken`
- [ ] Retornan entidades de Domain (no DTOs)
- [ ] NO tienen método `SaveChangesAsync()`
- [ ] NO exponen `DbContext` o tipos de EF Core

---

### ✅ 2.3 Unit of Work Interface

```csharp
// ✅ CORRECTO: src\Sports.Domain\Interfaces\IUnitOfWork.cs
namespace Sports.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // ✅ Propiedades para acceder a repositorios
    ITeamRepository Teams { get; }
    IPlayerRepository Players { get; }
    IMatchRepository Matches { get; }
    IMatchStatisticRepository MatchStatistics { get; }
    
    // ✅ Único punto de SaveChanges
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    // ✅ Gestión de transacciones
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

**Checklist:**
- [ ] Hereda de `IDisposable`
- [ ] Expone todos los repositorios como propiedades
- [ ] Tiene método `SaveChangesAsync()` (único punto de guardado)
- [ ] Implementa gestión de transacciones
- [ ] NO tiene referencias a EF Core

---

### ✅ 2.4 Enums

```csharp
// ✅ CORRECTO: src\Sports.Domain\Enums\SportType.cs
namespace Sports.Domain.Enums;

public enum SportType
{
    Football = 1,
    Basketball = 2,
    Tennis = 3,
    Baseball = 4,
    Volleyball = 5
}

// ✅ CORRECTO: src\Sports.Domain\Enums\Position.cs
namespace Sports.Domain.Enums;

public enum Position
{
    Goalkeeper = 1,
    Defender = 2,
    Midfielder = 3,
    Forward = 4
}
```

**Checklist:**
- [ ] Enums con valores explícitos
- [ ] Nombres descriptivos en inglés
- [ ] Sin Data Annotations de EF

---

## 3. Capa Application

### ✅ 3.1 Dependency Injection

```csharp
// ✅ CORRECTO: src\Sports.Application\DependencyInjection.cs
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Sports.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ✅ AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        // ✅ MediatR
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        // ✅ FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
```

**Checklist:**
- [ ] Método estático `AddApplication(this IServiceCollection services)`
- [ ] Registra AutoMapper desde el Assembly actual
- [ ] Registra MediatR handlers desde el Assembly actual
- [ ] Registra validators de FluentValidation desde el Assembly actual
- [ ] NO registra servicios de Infrastructure (como repositorios)

---

### ✅ 3.2 Commands (CQRS)

#### Command DTO
```csharp
// ✅ CORRECTO: src\Sports.Application\Features\Teams\Commands\CreateTeam\CreateTeamCommand.cs
using MediatR;
using Sports.Domain.Enums;

namespace Sports.Application.Features.Teams.Commands.CreateTeam;

public record CreateTeamCommand(
    string Name,
    string ShortName,
    DateTime FoundedDate,
    string City,
    string Country,
    SportType SportType,
    string? Logo = null,
    string? Stadium = null
) : IRequest<Guid>;  // ✅ Retorna el Id del equipo creado
```

#### Command Handler
```csharp
// ✅ CORRECTO: src\Sports.Application\Features\Teams\Commands\CreateTeam\CreateTeamCommandHandler.cs
using MediatR;
using Sports.Domain.Entities;
using Sports.Domain.Interfaces;

namespace Sports.Application.Features.Teams.Commands.CreateTeam;

public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTeamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;  // ✅ Inyección de IUnitOfWork
    }

    public async Task<Guid> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        // ✅ 1. Crear entidad usando constructor de Domain
        var team = new Team(
            request.Name,
            request.ShortName,
            request.FoundedDate,
            request.City,
            request.Country,
            request.SportType,
            request.Logo,
            request.Stadium
        );

        // ✅ 2. Agregar a través del repositorio
        await _unitOfWork.Teams.AddAsync(team, cancellationToken);
        
        // ✅ 3. Guardar cambios a través de UnitOfWork
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return team.Id;
    }
}
```

#### ❌ Anti-patrones
```csharp
// ❌ INCORRECTO: Inyectar DbContext directamente
public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, Guid>
{
    private readonly SportsDbContext _context;  // ❌ NO usar DbContext

    public async Task<Guid> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        var team = new Team(...);
        _context.Teams.Add(team);  // ❌ NO acceso directo a DbContext
        await _context.SaveChangesAsync();  // ❌ NO SaveChanges aquí
        return team.Id;
    }
}

// ❌ INCORRECTO: Lógica de negocio en el handler
public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, Guid>
{
    public async Task<Guid> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        var team = new Team();
        team.Name = request.Name;  // ❌ Mutación directa
        team.IsActive = true;      // ❌ Lógica de negocio aquí
        
        await _unitOfWork.Teams.AddAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return team.Id;
    }
}
```

**Checklist:**
- [ ] Command es un `record` con `IRequest<TResponse>`
- [ ] Handler inyecta `IUnitOfWork` (NO `DbContext` directamente)
- [ ] Handler NO tiene lógica de negocio (usa métodos de entidades)
- [ ] Handler llama a `_unitOfWork.SaveChangesAsync()` al final
- [ ] Handler NO llama a `_repository.SaveChangesAsync()`

---

### ✅ 3.3 Queries (CQRS)

#### Query DTO
```csharp
// ✅ CORRECTO: src\Sports.Application\Features\Teams\Queries\GetAllTeams\GetAllTeamsQuery.cs
using MediatR;
using Sports.Application.DTOs;

namespace Sports.Application.Features.Teams.Queries.GetAllTeams;

public record GetAllTeamsQuery() : IRequest<IEnumerable<TeamDto>>;
```

#### Query Handler
```csharp
// ✅ CORRECTO: src\Sports.Application\Features\Teams\Queries\GetAllTeams\GetAllTeamsQueryHandler.cs
using AutoMapper;
using MediatR;
using Sports.Application.DTOs;
using Sports.Domain.Interfaces;

namespace Sports.Application.Features.Teams.Queries.GetAllTeams;

public class GetAllTeamsQueryHandler : IRequestHandler<GetAllTeamsQuery, IEnumerable<TeamDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllTeamsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TeamDto>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
    {
        // ✅ 1. Obtener datos del repositorio
        var teams = await _unitOfWork.Teams.GetAllAsync(cancellationToken);
        
        // ✅ 2. Mapear a DTOs usando AutoMapper
        return _mapper.Map<IEnumerable<TeamDto>>(teams);
    }
}
```

**Checklist:**
- [ ] Query es un `record` con `IRequest<TResponse>`
- [ ] Handler inyecta `IUnitOfWork` y `IMapper`
- [ ] Handler retorna DTOs (NO entidades de Domain)
- [ ] Handler usa AutoMapper para mapeo
- [ ] Query NO modifica estado (solo lectura)

---

### ✅ 3.4 DTOs

```csharp
// ✅ CORRECTO: src\Sports.Application\DTOs\TeamDto.cs
using Sports.Domain.Enums;

namespace Sports.Application.DTOs;

public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public DateTime FoundedDate { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Stadium { get; set; }
    public SportType SportType { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // ✅ DTOs pueden exponer colecciones relacionadas
    public ICollection<PlayerDto> Players { get; set; } = new List<PlayerDto>();
}
```

**Checklist:**
- [ ] DTOs tienen propiedades públicas con `get; set;`
- [ ] DTOs inicializan colecciones a listas vacías
- [ ] DTOs NO tienen lógica de negocio
- [ ] DTOs NO tienen referencias a entidades de Domain directamente expuestas

---

### ✅ 3.5 AutoMapper Profiles

```csharp
// ✅ CORRECTO: src\Sports.Application\Mappings\MappingProfile.cs
using AutoMapper;
using Sports.Application.DTOs;
using Sports.Domain.Entities;

namespace Sports.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ✅ Mapeo de entidades a DTOs
        CreateMap<Team, TeamDto>()
            .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players));

        CreateMap<Player, PlayerDto>();
        CreateMap<Match, MatchDto>();
        CreateMap<MatchStatistic, MatchStatisticDto>();
    }
}
```

**Checklist:**
- [ ] Profile hereda de `AutoMapper.Profile`
- [ ] Mapeos configurados en el constructor
- [ ] Mapeos de entidades → DTOs
- [ ] Configuraciones explícitas para propiedades complejas

---

### ✅ 3.6 FluentValidation Validators

```csharp
// ✅ CORRECTO: src\Sports.Application\Features\Teams\Commands\CreateTeam\CreateTeamCommandValidator.cs
using FluentValidation;

namespace Sports.Application.Features.Teams.Commands.CreateTeam;

public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required")
            .MaximumLength(100).WithMessage("Team name must not exceed 100 characters");

        RuleFor(x => x.ShortName)
            .NotEmpty().WithMessage("Short name is required")
            .MaximumLength(10).WithMessage("Short name must not exceed 10 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters");

        RuleFor(x => x.FoundedDate)
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Founded date cannot be in the future");

        RuleFor(x => x.SportType)
            .IsInEnum().WithMessage("Invalid sport type");
    }
}
```

**Checklist:**
- [ ] Validator hereda de `AbstractValidator<TCommand>`
- [ ] Reglas configuradas en el constructor
- [ ] Mensajes de error descriptivos
- [ ] Validaciones de longitud coinciden con configuraciones de EF

---

## 4. Capa Infrastructure

### ✅ 4.1 Dependency Injection

```csharp
// ✅ CORRECTO: src\Sports.Infrastructure\DependencyInjection.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sports.Domain.Interfaces;
using Sports.Infrastructure.Data;
using Sports.Infrastructure.Repositories;

namespace Sports.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // ✅ Registro de DbContext
        services.AddDbContext<SportsDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(SportsDbContext).Assembly.FullName)
                      .EnableRetryOnFailure(
                          maxRetryCount: 3,
                          maxRetryDelay: TimeSpan.FromSeconds(5),
                          errorNumbersToAdd: null)));

        // ✅ Registro de repositorios
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IMatchStatisticRepository, MatchStatisticRepository>();
        
        // ✅ Registro de UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
```

**Checklist:**
- [ ] Método estático `AddInfrastructure(IServiceCollection, IConfiguration)`
- [ ] Registra `DbContext` con SQL Server
- [ ] Especifica `MigrationsAssembly` en Infrastructure
- [ ] Configura retry policy para resiliencia
- [ ] Registra todos los repositorios como `Scoped`
- [ ] Registra `IUnitOfWork` como `Scoped`

---

### ✅ 4.2 DbContext

```csharp
// ✅ CORRECTO: src\Sports.Infrastructure\Data\SportsDbContext.cs
using Microsoft.EntityFrameworkCore;
using Sports.Domain.Entities;

namespace Sports.Infrastructure.Data;

public class SportsDbContext : DbContext
{
    public SportsDbContext(DbContextOptions<SportsDbContext> options) : base(options)
    {
    }

    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MatchStatistic> MatchStatistics => Set<MatchStatistic>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ Aplicar todas las configuraciones del assembly actual
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SportsDbContext).Assembly);
    }
}
```

**Checklist:**
- [ ] Hereda de `DbContext`
- [ ] Constructor recibe `DbContextOptions<TContext>`
- [ ] DbSets expuestos como propiedades
- [ ] `OnModelCreating` aplica configuraciones desde Assembly

---

### ✅ 4.3 Entity Configurations

```csharp
// ✅ CORRECTO: src\Sports.Infrastructure\Data\Configurations\TeamConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sports.Domain.Entities;

namespace Sports.Infrastructure.Data.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        // ✅ Primary Key
        builder.HasKey(t => t.Id);

        // ✅ Propiedades escalares
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.ShortName)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(t => t.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Logo)
            .HasMaxLength(500);

        builder.Property(t => t.Stadium)
            .HasMaxLength(200);

        // ✅ Conversión de Enum a string
        builder.Property(t => t.SportType)
            .IsRequired()
            .HasConversion<string>();

        // ✅ Relaciones
        builder.HasMany(t => t.Players)
            .WithOne(p => p.Team)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.HomeMatches)
            .WithOne(m => m.HomeTeam)
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.AwayMatches)
            .WithOne(m => m.AwayTeam)
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // ✅ Índices
        builder.HasIndex(t => t.Name);
        builder.HasIndex(t => t.ShortName);
    }
}
```

**Checklist:**
- [ ] Implementa `IEntityTypeConfiguration<TEntity>`
- [ ] Configura Primary Key
- [ ] Configura propiedades requeridas y longitudes
- [ ] Enums convertidos a string
- [ ] Relaciones configuradas correctamente
- [ ] Foreign Keys explícitas
- [ ] Delete Behavior configurado
- [ ] Índices creados para búsquedas frecuentes
- [ ] NO usa Data Annotations en entidades

---

### ✅ 4.4 Repository Implementation

```csharp
// ✅ CORRECTO: src\Sports.Infrastructure\Repositories\TeamRepository.cs
using Microsoft.EntityFrameworkCore;
using Sports.Domain.Entities;
using Sports.Domain.Enums;
using Sports.Domain.Interfaces;
using Sports.Infrastructure.Data;

namespace Sports.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly SportsDbContext _context;

    public TeamRepository(SportsDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Team>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .ToListAsync(cancellationToken);
    }

    public async Task<Team> AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        await _context.Teams.AddAsync(team, cancellationToken);
        return team;  // ✅ NO llama a SaveChangesAsync()
    }

    public Task UpdateAsync(Team team, CancellationToken cancellationToken = default)
    {
        _context.Teams.Update(team);
        return Task.CompletedTask;  // ✅ NO llama a SaveChangesAsync()
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var team = await _context.Teams.FindAsync(new object[] { id }, cancellationToken);
        if (team != null)
        {
            _context.Teams.Remove(team);
            // ✅ NO llama a SaveChangesAsync()
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Teams.AnyAsync(t => t.Id == id, cancellationToken);
    }
}
```

#### ❌ Anti-patrones
```csharp
// ❌ INCORRECTO: Llamar a SaveChangesAsync en repositorio
public async Task<Team> AddAsync(Team team, CancellationToken cancellationToken = default)
{
    await _context.Teams.AddAsync(team, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);  // ❌ NO hacer esto
    return team;
}

// ❌ INCORRECTO: Devolver Task<int> desde Add
public async Task<int> AddAsync(Team team, CancellationToken cancellationToken = default)
{
    await _context.Teams.AddAsync(team, cancellationToken);
    return await _context.SaveChangesAsync(cancellationToken);  // ❌ NO
}
```

**Checklist:**
- [ ] Repositorio implementa interfaz de Domain
- [ ] Inyecta `DbContext`
- [ ] Usa `Include()` para cargar entidades relacionadas
- [ ] **NO llama a `SaveChangesAsync()`** (responsabilidad de UnitOfWork)
- [ ] Métodos async con `CancellationToken`
- [ ] `Update()` retorna `Task.CompletedTask`
- [ ] `Delete()` NO lanza excepción si no existe (soft delete)

---

### ✅ 4.5 Unit of Work Implementation

```csharp
// ✅ CORRECTO: src\Sports.Infrastructure\Repositories\UnitOfWork.cs
using Microsoft.EntityFrameworkCore.Storage;
using Sports.Domain.Interfaces;
using Sports.Infrastructure.Data;

namespace Sports.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SportsDbContext _context;
    private IDbContextTransaction? _transaction;

    // ✅ Propiedades de repositorios
    public ITeamRepository Teams { get; }
    public IPlayerRepository Players { get; }
    public IMatchRepository Matches { get; }
    public IMatchStatisticRepository MatchStatistics { get; }

    // ✅ Constructor inyecta DbContext y repositorios
    public UnitOfWork(
        SportsDbContext context,
        ITeamRepository teams,
        IPlayerRepository players,
        IMatchRepository matches,
        IMatchStatisticRepository matchStatistics)
    {
        _context = context;
        Teams = teams;
        Players = players;
        Matches = matches;
        MatchStatistics = matchStatistics;
    }

    // ✅ Único punto de SaveChanges
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // ✅ Gestión de transacciones
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
```

**Checklist:**
- [ ] Implementa `IUnitOfWork` de Domain
- [ ] Inyecta `DbContext` y todos los repositorios
- [ ] Expone repositorios como propiedades readonly
- [ ] `SaveChangesAsync()` llama a `_context.SaveChangesAsync()`
- [ ] Gestiona transacciones correctamente
- [ ] `CommitTransactionAsync()` llama a `SaveChangesAsync()` antes de commit
- [ ] `Dispose()` limpia transacción y contexto

---

### ✅ 4.6 Migrations

```csharp
// ✅ CORRECTO: Migrations generadas automáticamente
// src\Sports.Infrastructure\Migrations\20251230204502_InitialCreate.cs

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // ✅ Código generado automáticamente por EF Core Tools
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // ✅ Código generado automáticamente por EF Core Tools
    }
}
```

**Comandos:**
```bash
# Crear migration
dotnet ef migrations add InitialCreate --project src/Sports.Infrastructure --startup-project src/Sports.API

# Aplicar migration
dotnet ef database update --project src/Sports.Infrastructure --startup-project src/Sports.API

# Remover última migration (si no aplicada)
dotnet ef migrations remove --project src/Sports.Infrastructure --startup-project src/Sports.API
```

**Checklist:**
- [ ] Migrations en carpeta `Migrations/` de Infrastructure
- [ ] `MigrationsAssembly` configurado en `AddInfrastructure()`
- [ ] Migrations aplicadas automáticamente en `Program.cs`
- [ ] NO modificar migrations manualmente (regenerar si necesario)

---

## 5. Capa API

### ✅ 5.1 Controllers

```csharp
// ✅ CORRECTO: src\Sports.API\Controllers\TeamsController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sports.Application.Features.Teams.Commands.CreateTeam;
using Sports.Application.Features.Teams.Queries.GetAllTeams;
using Sports.Application.Features.Teams.Queries.GetTeamById;

namespace Sports.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamsController(IMediator mediator)
    {
        _mediator = mediator;  // ✅ Solo inyecta IMediator
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllTeamsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTeamByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
            return NotFound();
        
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTeamCommand command, 
        CancellationToken cancellationToken)
    {
        var teamId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = teamId }, teamId);
    }
}
```

#### ❌ Anti-patrones
```csharp
// ❌ INCORRECTO: Lógica de negocio en el controller
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateTeamCommand command)
{
    var team = new Team(command.Name, command.ShortName, ...);  // ❌ NO
    await _unitOfWork.Teams.AddAsync(team);                     // ❌ NO
    await _unitOfWork.SaveChangesAsync();                       // ❌ NO
    return Ok(team.Id);
}

// ❌ INCORRECTO: Inyectar DbContext
public class TeamsController : ControllerBase
{
    private readonly SportsDbContext _context;  // ❌ NO
}

// ❌ INCORRECTO: Inyectar repositorios directamente
public class TeamsController : ControllerBase
{
    private readonly ITeamRepository _teamRepository;  // ❌ NO
}
```

**Checklist:**
- [ ] Controller solo inyecta `IMediator`
- [ ] Métodos solo dispatch a MediatR
- [ ] NO contiene lógica de negocio
- [ ] NO inyecta `DbContext` directamente
- [ ] NO inyecta repositorios directamente
- [ ] Retorna códigos HTTP apropiados (`Ok`, `NotFound`, `CreatedAtAction`)
- [ ] Acepta `CancellationToken` en métodos async

---

### ✅ 5.2 Program.cs

```csharp
// ✅ CORRECTO: src\Sports.API\Program.cs
using Microsoft.EntityFrameworkCore;
using Sports.Application;
using Sports.Infrastructure;
using Sports.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ Servicios API
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ Capas Application e Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// ✅ Aplicar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SportsDbContext>();
    context.Database.Migrate();
}

// ✅ Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAngular");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

#### ❌ Anti-patrones
```csharp
// ❌ INCORRECTO: Registrar servicios manualmente
builder.Services.AddScoped<ITeamRepository, TeamRepository>();  // ❌ Debe estar en DependencyInjection.cs
builder.Services.AddDbContext<SportsDbContext>(...);             // ❌ Debe estar en DependencyInjection.cs

// ❌ INCORRECTO: Lógica de negocio en Program.cs
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SportsDbContext>();
    var team = new Team("Example", "EX", ...);  // ❌ NO
    context.Teams.Add(team);                    // ❌ NO
    await context.SaveChangesAsync();           // ❌ NO
}

// ❌ INCORRECTO: OpenAPI en producción
app.MapOpenApi();  // ❌ Solo en Development
```

**Checklist:**
- [ ] `Program.cs` es minimal y solo orquesta
- [ ] Llama a `AddApplication()` y `AddInfrastructure()`
- [ ] Aplica migraciones en startup con `context.Database.Migrate()`
- [ ] OpenAPI solo en Development
- [ ] CORS configurado antes de otros middlewares
- [ ] HTTPS redirection habilitado
- [ ] NO registra servicios manualmente
- [ ] NO contiene lógica de negocio

---

## 6. Dependency Injection

### ✅ Patrón Global

```
Program.cs
   ├── AddApplication()           [Sports.Application]
   │     ├── AutoMapper
   │     ├── MediatR
   │     └── FluentValidation
   │
   └── AddInfrastructure(config)  [Sports.Infrastructure]
         ├── DbContext
         ├── Repositories (Scoped)
         └── UnitOfWork (Scoped)
```

**Checklist:**
- [ ] Cada capa tiene su propio `DependencyInjection.cs`
- [ ] Application no registra servicios de Infrastructure
- [ ] Infrastructure no registra servicios de Application
- [ ] Todos los servicios son `Scoped` (lifetime correcta para web)
- [ ] Program.cs solo llama a métodos de extensión

---

## 7. Patrón Repository

### ✅ Principios

1. **Interface en Domain, Implementación en Infrastructure**
```
Sports.Domain\Interfaces\ITeamRepository.cs        [Interfaz]
Sports.Infrastructure\Repositories\TeamRepository.cs [Implementación]
```

2. **NO llamar a SaveChanges en repositorios**
```csharp
// ✅ CORRECTO
public async Task<Team> AddAsync(Team team, CancellationToken cancellationToken)
{
    await _context.Teams.AddAsync(team, cancellationToken);
    return team;  // ✅ NO SaveChanges
}

// ❌ INCORRECTO
public async Task<Team> AddAsync(Team team, CancellationToken cancellationToken)
{
    await _context.Teams.AddAsync(team, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);  // ❌ NO hacer esto
    return team;
}
```

3. **Retornar entidades de Domain**
```csharp
// ✅ CORRECTO
Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

// ❌ INCORRECTO
Task<TeamDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
```

**Checklist:**
- [ ] Interfaces en Domain, implementaciones en Infrastructure
- [ ] Repositorios NO llaman a `SaveChangesAsync()`
- [ ] Repositorios retornan entidades (no DTOs)
- [ ] Métodos async con `CancellationToken`
- [ ] Repositorios NO exponen `DbContext`

---

## 8. Unit of Work

### ✅ Responsabilidades

1. **Único punto de SaveChanges**
```csharp
// ✅ Handler usa UnitOfWork.SaveChangesAsync()
public async Task<Guid> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
{
    var team = new Team(...);
    await _unitOfWork.Teams.AddAsync(team, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);  // ✅ Aquí
    return team.Id;
}
```

2. **Gestión de transacciones**
```csharp
// ✅ Ejemplo con transacción
await _unitOfWork.BeginTransactionAsync(cancellationToken);
try
{
    await _unitOfWork.Teams.AddAsync(team, cancellationToken);
    await _unitOfWork.Players.AddAsync(player, cancellationToken);
    await _unitOfWork.CommitTransactionAsync(cancellationToken);
}
catch
{
    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
    throw;
}
```

**Checklist:**
- [ ] Handlers inyectan `IUnitOfWork` (no repositorios individuales)
- [ ] Handlers llaman a `_unitOfWork.SaveChangesAsync()` al final
- [ ] Transacciones usadas para operaciones multi-entidad
- [ ] UnitOfWork implementa `IDisposable`

---

## 9. CQRS con MediatR

### ✅ Estructura

```
Features/
├── Teams/
│   ├── Commands/
│   │   └── CreateTeam/
│   │       ├── CreateTeamCommand.cs
│   │       ├── CreateTeamCommandHandler.cs
│   │       └── CreateTeamCommandValidator.cs
│   └── Queries/
│       ├── GetAllTeams/
│       │   ├── GetAllTeamsQuery.cs
│       │   └── GetAllTeamsQueryHandler.cs
│       └── GetTeamById/
│           ├── GetTeamByIdQuery.cs
│           └── GetTeamByIdQueryHandler.cs
```

**Checklist:**
- [ ] Estructura por Feature (vertical slicing)
- [ ] Commands y Queries separados
- [ ] Cada comando tiene Command, Handler y Validator
- [ ] Handlers implementan `IRequestHandler<TRequest, TResponse>`
- [ ] Controllers solo despachan a MediatR

---

## 10. Validación con FluentValidation

### ✅ Reglas

```csharp
public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        // ✅ Validaciones de negocio
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required")
            .MaximumLength(100);  // ✅ Coincide con EF config

        RuleFor(x => x.FoundedDate)
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Founded date cannot be in the future");

        RuleFor(x => x.SportType)
            .IsInEnum();
    }
}
```

**Checklist:**
- [ ] Validators heredan de `AbstractValidator<TCommand>`
- [ ] Longitudes máximas coinciden con EF Configurations
- [ ] Mensajes de error descriptivos
- [ ] Validaciones de negocio (no solo formato)
- [ ] Registrados automáticamente en `AddApplication()`

---

## 11. Entity Framework Core

### ✅ Configuraciones

1. **DbContext**
   - [ ] Hereda de `DbContext`
   - [ ] DbSets como propiedades
   - [ ] `ApplyConfigurationsFromAssembly()` en `OnModelCreating`

2. **Entity Configurations**
   - [ ] Implementan `IEntityTypeConfiguration<TEntity>`
   - [ ] Configuran Primary Keys
   - [ ] Configuran longitudes y tipos
   - [ ] Configuran relaciones y Foreign Keys
   - [ ] Configuran Delete Behaviors
   - [ ] Crean índices

3. **Migrations**
   - [ ] Generadas en Infrastructure
   - [ ] Aplicadas automáticamente en startup
   - [ ] NO modificadas manualmente

4. **Conversiones**
   - [ ] Enums convertidos a string: `.HasConversion<string>()`

---

## 12. Program.cs y Middleware Pipeline

### ✅ Orden Correcto de Middleware

```csharp
var app = builder.Build();

// 1. Aplicar migraciones
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SportsDbContext>();
    context.Database.Migrate();
}

// 2. Development-only
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 3. CORS (antes de otros)
app.UseCors("AllowAngular");

// 4. HTTPS redirection
app.UseHttpsRedirection();

// 5. Authentication/Authorization
app.UseAuthorization();

// 6. Endpoints
app.MapControllers();

app.Run();
```

**Checklist:**
- [ ] Migraciones aplicadas en startup
- [ ] OpenAPI solo en Development
- [ ] CORS antes de otros middlewares
- [ ] HTTPS redirection habilitado
- [ ] Orden correcto: CORS → HTTPS → Authorization → Endpoints

---

## 📊 Resumen de Verificación

### Checklist Rápido de Arquitectura

#### Domain Layer
- [ ] NO referencia otros proyectos
- [ ] NO referencia EF Core, MediatR, ASP.NET
- [ ] Entidades con `private set` y encapsulación
- [ ] Interfaces de repositorio sin SaveChanges
- [ ] IUnitOfWork con SaveChangesAsync

#### Application Layer
- [ ] Solo referencia Domain
- [ ] CQRS: Commands y Queries separados
- [ ] Handlers usan IUnitOfWork
- [ ] DTOs para transferencia
- [ ] AutoMapper para mapeo
- [ ] FluentValidation para validación
- [ ] DependencyInjection.AddApplication()

#### Infrastructure Layer
- [ ] Referencia Domain y Application
- [ ] DbContext en Infrastructure
- [ ] Entity Configurations separadas
- [ ] Repositorios NO llaman a SaveChanges
- [ ] UnitOfWork implementa IUnitOfWork
- [ ] DependencyInjection.AddInfrastructure(config)

#### API Layer
- [ ] Referencia Application e Infrastructure
- [ ] Controllers solo despachan a MediatR
- [ ] NO contienen lógica de negocio
- [ ] Program.cs minimal y limpio
- [ ] Migraciones automáticas en startup
- [ ] OpenAPI solo en Development

---

## 🔍 Comandos de Verificación

```bash
# Verificar dependencias de proyectos
dotnet list src/Sports.Domain/Sports.Domain.csproj package
dotnet list src/Sports.Application/Sports.Application.csproj package
dotnet list src/Sports.Infrastructure/Sports.Infrastructure.csproj package
dotnet list src/Sports.API/Sports.API.csproj package

# Verificar referencias entre proyectos
dotnet list src/Sports.Domain/Sports.Domain.csproj reference
dotnet list src/Sports.Application/Sports.Application.csproj reference
dotnet list src/Sports.Infrastructure/Sports.Infrastructure.csproj reference
dotnet list src/Sports.API/Sports.API.csproj reference

# Build completo
dotnet build Sports.sln

# Ejecutar tests
dotnet test Sports.sln

# Crear migration
dotnet ef migrations add MigrationName --project src/Sports.Infrastructure --startup-project src/Sports.API

# Aplicar migration
dotnet ef database update --project src/Sports.Infrastructure --startup-project src/Sports.API

# Verificar DbContext
dotnet ef dbcontext info --project src/Sports.Infrastructure --startup-project src/Sports.API
```

---

## ✅ Conclusión

Este checklist debe usarse para:
1. **Validar** arquitectura de nuevas features
2. **Revisar** código en Pull Requests
3. **Refactorizar** código que no cumpla las reglas
4. **Entrenar** nuevos desarrolladores en el equipo

**Regla de oro:** Si tienes dudas, vuelve a `01-architecture.md` y verifica que tu implementación siga los principios de Clean Architecture.
