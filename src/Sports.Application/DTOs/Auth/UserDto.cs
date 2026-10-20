namespace Sports.Application.DTOs.Auth;

/// <summary>
/// User data transfer object for responses.
/// PascalCase properties for frontend consistency.
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = new();
}
