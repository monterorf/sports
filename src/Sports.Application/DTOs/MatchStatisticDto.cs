namespace Sports.Application.DTOs;

public class MatchStatisticDto
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public int MinutesPlayed { get; set; }
    public int Shots { get; set; }
    public int ShotsOnTarget { get; set; }
    public int Passes { get; set; }
    public int PassesCompleted { get; set; }
    public int Fouls { get; set; }
    public int FoulsReceived { get; set; }
    public double PassAccuracy { get; set; }
    public double ShotAccuracy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
