namespace Sports.Application.DTOs;

public class MatchDto
{
    public Guid Id { get; set; }
    public Guid HomeTeamId { get; set; }
    public string HomeTeamName { get; set; } = string.Empty;
    public Guid AwayTeamId { get; set; }
    public string AwayTeamName { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string? Competition { get; set; }
    public string? Season { get; set; }
    public int HomeTeamScore { get; set; }
    public int AwayTeamScore { get; set; }
    public string Status { get; set; } = string.Empty;
    public string SportType { get; set; } = string.Empty;
    public string Winner { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
