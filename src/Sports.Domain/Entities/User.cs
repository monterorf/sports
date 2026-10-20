namespace Sports.Domain.Entities;

/// <summary>
/// User entity for authentication and authorization.
/// Supports multiple roles per user.
/// </summary>
public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// BCrypt hashed password. NEVER store plaintext passwords.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// List of roles assigned to this user.
    /// Examples: "Admin", "LeagueManager", "TeamManager", "Player", "Spectator"
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Foreign key to Team (if user is Player or TeamManager).
    /// Null if user is not associated with a team.
    /// </summary>
    public Guid? TeamId { get; set; }

    /// <summary>
    /// Foreign key to League (if user is LeagueManager).
    /// Null if user is not associated with a league.
    /// </summary>
    public Guid? LeagueId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Logical delete flag. False = inactive/deleted user.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
