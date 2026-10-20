using Moq;
using Xunit;
using FluentAssertions;
using Sports.API.Controllers;
using Sports.Application.DTOs.Auth;
using Sports.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Sports.API.Tests.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    #region Login Tests

    [Fact]
    public async Task Login_WithValidRequest_ReturnsOkWithToken()
    {
        // Arrange
        var request = new LoginRequest { Email = "user@example.com", Password = "Password123!" };
        var response = new AuthResponseDto
        {
            Success = true,
            Message = "Login exitoso",
            Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
            User = new UserDto
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                Roles = new List<string> { "Player" }
            }
        };

        _mockAuthService
            .Setup(x => x.LoginAsync(request, CancellationToken.None))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Login(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(response);
        _mockAuthService.Verify(x => x.LoginAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var request = new LoginRequest { Email = "invalid@example.com", Password = "WrongPassword" };
        var response = new AuthResponseDto
        {
            Success = false,
            Message = "Email o contraseña inválidos"
        };

        _mockAuthService
            .Setup(x => x.LoginAsync(request, CancellationToken.None))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Login(request, CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badResult.StatusCode.Should().Be(400);
        badResult.Value.Should().Be(response);
    }

    #endregion

    #region Register Tests

    [Fact]
    public async Task Register_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            Password = "SecurePass123!"
        };
        var response = new AuthResponseDto
        {
            Success = true,
            Message = "Registro exitoso",
            Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
            User = new UserDto
            {
                Id = Guid.NewGuid(),
                Email = "newuser@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                Roles = new List<string> { "Player" }
            }
        };

        _mockAuthService
            .Setup(x => x.RegisterAsync(request, CancellationToken.None))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Register(request, CancellationToken.None);

        // Assert
        var createdResult = result.Should().BeOfType<ObjectResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.Value.Should().Be(response);
        _mockAuthService.Verify(x => x.RegisterAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "existing@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "SecurePass123!"
        };
        var response = new AuthResponseDto
        {
            Success = false,
            Message = "El email ya está registrado"
        };

        _mockAuthService
            .Setup(x => x.RegisterAsync(request, CancellationToken.None))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Register(request, CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badResult.StatusCode.Should().Be(400);
        badResult.Value.Should().Be(response);
    }

    [Fact]
    public async Task Register_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "invalid-email",
            FirstName = "X",
            LastName = "Y",
            Password = "weak"
        };
        var response = new AuthResponseDto
        {
            Success = false,
            Message = "Validación fallida: Email debe ser válido, FirstName debe tener al menos 2 caracteres"
        };

        _mockAuthService
            .Setup(x => x.RegisterAsync(request, CancellationToken.None))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Register(request, CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badResult.StatusCode.Should().Be(400);
        badResult.Value.Should().Be(response);
    }

    #endregion

    #region HTTP Status Code Tests

    [Fact]
    public async Task Login_SuccessfulLogin_ReturnsStatusCode200()
    {
        // Arrange
        var request = new LoginRequest { Email = "user@example.com", Password = "Password123!" };
        var response = new AuthResponseDto { Success = true, Token = "token..." };

        _mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Login(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result).StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Register_SuccessfulRegistration_ReturnsStatusCode201()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "new@example.com",
            FirstName = "Jane",
            LastName = "Doe",
            Password = "SecurePass123!"
        };
        var response = new AuthResponseDto { Success = true, Token = "token..." };

        _mockAuthService
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Register(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        ((ObjectResult)result).StatusCode.Should().Be(201);
    }

    #endregion
}
