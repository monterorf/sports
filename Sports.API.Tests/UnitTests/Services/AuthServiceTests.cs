using Moq;
using Xunit;
using FluentAssertions;
using Sports.Application.DTOs.Auth;
using Sports.Application.Interfaces;
using Sports.Application.Services;
using Sports.Domain.Entities;
using Sports.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace Sports.API.Tests.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IValidator<LoginRequest>> _mockLoginValidator;
    private readonly Mock<IValidator<RegisterRequest>> _mockRegisterValidator;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLoginValidator = new Mock<IValidator<LoginRequest>>();
        _mockRegisterValidator = new Mock<IValidator<RegisterRequest>>();
        _mockConfiguration = new Mock<IConfiguration>();

        // Setup configuration defaults
        SetupConfigurationMocks();

        _authService = new AuthService(
            _mockUserRepository.Object,
            _mockLoginValidator.Object,
            _mockRegisterValidator.Object,
            _mockConfiguration.Object);
    }

    private void SetupConfigurationMocks()
    {
        _mockConfiguration.Setup(x => x["Jwt:Key"])
            .Returns("this-is-a-very-long-secret-key-that-is-at-least-32-chars-long-1234567890");

        _mockConfiguration.Setup(x => x["Jwt:Issuer"])
            .Returns("SportsAPI");

        _mockConfiguration.Setup(x => x["Jwt:Audience"])
            .Returns("SportsApp");

        _mockConfiguration.Setup(x => x["Jwt:ExpirationMinutes"])
            .Returns("60");
    }

    #region Login Tests

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccessWithToken()
    {
        // Arrange
        var request = new LoginRequest { Email = "user@example.com", Password = "Password123!" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Roles = new List<string> { "Player" },
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockLoginValidator
            .Setup(x => x.ValidateAsync(request, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(request.Email, CancellationToken.None))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be("user@example.com");
        result.User.Roles.Should().Contain("Player");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ReturnsFailed()
    {
        // Arrange
        var request = new LoginRequest { Email = "invalid@example.com", Password = "Password123!" };
        var validationErrors = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Email", "Invalid email format")
        };

        _mockLoginValidator
            .Setup(x => x.ValidateAsync(request, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Validación fallida");
        result.Token.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsFailed()
    {
        // Arrange
        var request = new LoginRequest { Email = "user@example.com", Password = "WrongPassword123!" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            Roles = new List<string> { "Player" },
            IsActive = true
        };

        _mockLoginValidator
            .Setup(x => x.ValidateAsync(request, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(request.Email, CancellationToken.None))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email o contraseña inválidos");
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ReturnsFailed()
    {
        // Arrange
        var request = new LoginRequest { Email = "inactive@example.com", Password = "Password123!" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "inactive@example.com",
            IsActive = false
        };

        _mockLoginValidator
            .Setup(x => x.ValidateAsync(request, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(request.Email, CancellationToken.None))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email o contraseña inválidos");
    }

    #endregion

    #region Register Tests

    [Fact]
    public async Task RegisterAsync_WithValidData_ReturnsSuccessWithToken()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            Password = "SecurePass123!"
        };

        _mockRegisterValidator
            .Setup(x => x.ValidateAsync(request, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockUserRepository
            .Setup(x => x.EmailExistsAsync(request.Email, CancellationToken.None))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be("newuser@example.com");
        result.User.Roles.Should().Contain("Player");

        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsFailed()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "existing@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "SecurePass123!"
        };

        _mockRegisterValidator
            .Setup(x => x.ValidateAsync(request, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockUserRepository
            .Setup(x => x.EmailExistsAsync(request.Email, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("El email ya está registrado");
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidPassword_ReturnsFailed()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "weak"
        };

        var validationErrors = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Password", "Password must be at least 8 characters")
        };

        _mockRegisterValidator
            .Setup(x => x.ValidateAsync(request, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Validación fallida");
    }

    #endregion

    #region Password Tests

    [Fact]
    public async Task HashPasswordAsync_ReturnsHashedPassword()
    {
        // Arrange
        var password = "MySecurePassword123!";

        // Act
        var hash = await _authService.HashPasswordAsync(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        // BCrypt hashes start with $2a$, $2b$, or $2y$ and are at least 60 chars
        hash.Length.Should().BeGreaterThanOrEqualTo(60);
        hash.Should().Contain("$");
    }

    [Fact]
    public async Task VerifyPasswordAsync_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "MySecurePassword123!";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);

        // Act
        var result = await _authService.VerifyPasswordAsync(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyPasswordAsync_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var password = "MySecurePassword123!";
        var wrongPassword = "WrongPassword123!";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);

        // Act
        var result = await _authService.VerifyPasswordAsync(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region JWT Tests

    [Fact]
    public async Task GenerateJwtAsync_WithValidUser_ReturnsValidToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            Roles = new List<string> { "Player", "TeamManager" },
            TeamId = Guid.NewGuid(),
            LeagueId = Guid.NewGuid()
        };

        // Act
        var token = await _authService.GenerateJwtAsync(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        // JWT format: header.payload.signature
        var parts = token.Split('.');
        parts.Should().HaveCount(3);
    }

    #endregion
}
