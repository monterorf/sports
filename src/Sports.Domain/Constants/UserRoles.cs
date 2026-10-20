namespace Sports.Domain.Constants;

/// <summary>
/// Defines all available user roles in the Sports application.
/// These roles are used for authorization attributes and permission checks.
/// </summary>
public static class UserRoles
{
    /// <summary>
    /// Admin: Full control over the entire application.
    /// Can create/edit/delete leagues, teams, matches, and manage users.
    /// </summary>
    public const string Admin = nameof(Admin);

    /// <summary>
    /// LeagueManager: Manages a specific league.
    /// Can create/edit teams, matches, and schedules within their league.
    /// </summary>
    public const string LeagueManager = nameof(LeagueManager);

    /// <summary>
    /// TeamManager: Manages a specific team.
    /// Can edit team details, manage players, and view match statistics.
    /// </summary>
    public const string TeamManager = nameof(TeamManager);

    /// <summary>
    /// Player: Registered player in the system.
    /// Can view their own profile, team info, and match statistics.
    /// Limited: cannot create or modify teams/matches.
    /// </summary>
    public const string Player = nameof(Player);

    /// <summary>
    /// Spectator: Public user without login (or minimal registration).
    /// Can view public match results, standings, and statistics.
    /// Cannot modify any data.
    /// </summary>
    public const string Spectator = nameof(Spectator);
}
