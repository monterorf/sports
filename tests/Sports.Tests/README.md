# Sports.Tests

Proyecto consolidado de pruebas para la aplicación Sports Management API.

## ?? Estructura

```
Sports.Tests/
??? Unit/                           # Tests unitarios (sin dependencias externas)
?   ??? Domain/                     # Tests de entidades y lógica de dominio
?   ?   ??? Entities/
?   ?   ??? ValueObjects/
?   ??? Application/                # Tests de handlers, validators, mappers
?       ??? Commands/
?       ??? Queries/
?       ??? Validators/
??? Integration/                    # Tests de integración (con BD, repos, etc)
?   ??? Repositories/              # Tests de repositorios con InMemory DB
?   ??? Database/                  # Tests de configuración EF Core
?   ??? Infrastructure/            # Tests de servicios de infraestructura
??? Functional/                     # Tests end-to-end (API completa)
    ??? API/                       # Tests de endpoints HTTP
```

## ?? Tipos de Tests

### Unit Tests (Rápidos, ~ms)
- **Propósito**: Probar lógica de negocio aislada
- **Sin dependencias**: Sin BD, sin HTTP, sin I/O
- **Ejemplos**:
  - Validación de entidades del dominio
  - Lógica de negocio en entidades
  - Validators de FluentValidation
  - AutoMapper profiles

### Integration Tests (Medios, ~100-500ms)
- **Propósito**: Probar componentes trabajando juntos
- **Con dependencias controladas**: InMemory DB, mocks
- **Ejemplos**:
  - Repositorios con Entity Framework
  - Configuración de DbContext
  - Queries LINQ complejas

### Functional Tests (Lentos, ~1-5s)
- **Propósito**: Probar la aplicación completa
- **End-to-End**: API real, flujos completos
- **Ejemplos**:
  - Endpoints HTTP
  - Flujos de autenticación
  - Validación de respuestas API

## ?? Ejecutar Tests

### Todos los tests
```bash
dotnet test
```

### Solo tests unitarios (rápidos)
```bash
dotnet test --filter "FullyQualifiedName~Sports.Tests.Unit"
```

### Solo tests de integración
```bash
dotnet test --filter "FullyQualifiedName~Sports.Tests.Integration"
```

### Solo tests funcionales
```bash
dotnet test --filter "FullyQualifiedName~Sports.Tests.Functional"
```

### Con cobertura de código
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## ?? Cobertura de Código

El objetivo es mantener una cobertura mínima de:
- **Unit Tests**: 80%+ de la lógica de negocio
- **Integration Tests**: Cubrir todos los repositorios y queries
- **Functional Tests**: Cubrir los principales flujos de usuario

## ??? Herramientas

- **xUnit**: Framework de testing
- **FluentAssertions**: Assertions expresivas
- **Moq**: Mocking framework
- **InMemory Database**: Para tests de integración
- **Coverlet**: Cobertura de código

## ?? Convenciones

### Nomenclatura
- Formato: `[MethodName]_Should[ExpectedBehavior]_When[Condition]`
- Ejemplo: `GetByIdAsync_ShouldReturnTeam_WhenTeamExists`

### Estructura AAA (Arrange-Act-Assert)
```csharp
[Fact]
public async Task Method_ShouldBehavior_WhenCondition()
{
    // Arrange
    // ... setup
    
    // Act
    var result = await _sut.Method();
    
    // Assert
    result.Should().NotBeNull();
}
```

### Test Data Builders
Para objetos complejos, usar builders o factory methods en carpeta `Builders/`.
