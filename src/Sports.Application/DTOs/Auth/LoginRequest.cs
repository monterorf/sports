namespace Sports.Application.DTOs.Auth;

/// <summary>
/// Login request DTO (PascalCase for consistency with frontend).
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
