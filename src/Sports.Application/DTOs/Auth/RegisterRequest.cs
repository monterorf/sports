namespace Sports.Application.DTOs.Auth;

/// <summary>
/// Register request DTO (PascalCase for consistency with frontend).
/// </summary>
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}
