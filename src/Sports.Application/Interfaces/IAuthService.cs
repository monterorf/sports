using Sports.Application.DTOs.Auth;
using Sports.Domain.Entities;

namespace Sports.Application.Interfaces;

/// <summary>
/// Service interface for authentication operations.
/// Handles user login, registration, and JWT token generation.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates user with email and password.
    /// Returns JWT token if credentials are valid.
    /// </summary>
    Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user with email, password, and name.
    /// New users are assigned "Player" role by default.
    /// </summary>
    Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates JWT token for authenticated user.
    /// Includes claims: sub (userId), email, roles, TeamId, LeagueId.
    /// </summary>
    Task<string> GenerateJwtAsync(User user);

    /// <summary>
    /// Verifies password against stored hash using BCrypt.
    /// </summary>
    Task<bool> VerifyPasswordAsync(string password, string hash);

    /// <summary>
    /// Hashes password using BCrypt.
    /// Stores safe hash in PasswordHash field.
    /// </summary>
    Task<string> HashPasswordAsync(string password);
}
