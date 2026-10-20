using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using Sports.Application.DTOs.Auth;
using Sports.Application.Interfaces;
using Sports.Domain.Entities;
using Sports.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;

namespace Sports.Application.Services;

/// <summary>
/// Service for authentication operations.
/// Handles login, registration, JWT generation, and password hashing.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IValidator<LoginRequest> loginValidator,
        IValidator<RegisterRequest> registerValidator,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _configuration = configuration;
    }

    /// <summary>
    /// Authenticates user with email and password.
    /// </summary>
    public async Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Validate request
        var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage));
            return new AuthResponseDto
            {
                Success = false,
                Message = $"Validación fallida: {errors}"
            };
        }

        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Email o contraseña inválidos"
            };
        }

        // Verify password
        var passwordValid = await VerifyPasswordAsync(request.Password, user.PasswordHash);
        if (!passwordValid)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Email o contraseña inválidos"
            };
        }

        // Generate JWT token
        var token = await GenerateJwtAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login exitoso",
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.Roles
            }
        };
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Validate request
        var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage));
            return new AuthResponseDto
            {
                Success = false,
                Message = $"Validación fallida: {errors}"
            };
        }

        // Check if email already exists
        var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);

        if (emailExists)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "El email ya está registrado"
            };
        }

        // Hash password
        var passwordHash = await HashPasswordAsync(request.Password);

        // Create new user with default "Player" role
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = passwordHash,
            Roles = new List<string> { "Player" },
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add user to database
        await _userRepository.AddAsync(user, cancellationToken);

        // Generate JWT token
        var token = await GenerateJwtAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Registro exitoso",
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.Roles
            }
        };
    }

    /// <summary>
    /// Generates JWT token with claims.
    /// </summary>
    public async Task<string> GenerateJwtAsync(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];
        var jwtExpirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key no está configurado en appsettings.json");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName)
        };

        // Add roles as multiple claims
        foreach (var role in user.Roles ?? new List<string>())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add custom claims for context
        if (user.TeamId.HasValue)
        {
            claims.Add(new Claim("TeamId", user.TeamId.ToString()));
        }

        if (user.LeagueId.HasValue)
        {
            claims.Add(new Claim("LeagueId", user.LeagueId.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtExpirationMinutes),
            signingCredentials: creds);

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    /// <summary>
    /// Verifies password using BCrypt.
    /// </summary>
    public async Task<bool> VerifyPasswordAsync(string password, string hash)
    {
        return await Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hash));
    }

    /// <summary>
    /// Hashes password using BCrypt.
    /// </summary>
    public async Task<string> HashPasswordAsync(string password)
    {
        return await Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password));
    }
}
