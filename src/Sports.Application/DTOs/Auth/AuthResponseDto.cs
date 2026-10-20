namespace Sports.Application.DTOs.Auth;

/// <summary>
/// Authentication response DTO.
/// Returned by Login and Register endpoints.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// Indicates if login/register was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Message for user feedback (success or error).
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// JWT token (if Success = true).
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// User data (if Success = true).
    /// </summary>
    public UserDto? User { get; set; }
}
