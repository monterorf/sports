using Sports.Domain.Common;

namespace Sports.Domain.Entities;

public class MatchStatistic : BaseEntity
{
    public Guid MatchId { get; private set; }
    public Match Match { get; private set; }

    public Guid PlayerId { get; private set; }
    public Player Player { get; private set; }

    public int Goals { get; private set; }
    public int Assists { get; private set; }
    public int YellowCards { get; private set; }
    public int RedCards { get; private set; }
    public int MinutesPlayed { get; private set; }
    public int Shots { get; private set; }
    public int ShotsOnTarget { get; private set; }
    public int Passes { get; private set; }
    public int PassesCompleted { get; private set; }
    public int Fouls { get; private set; }
    public int FoulsReceived { get; private set; }

    private MatchStatistic() { }

    public MatchStatistic(Guid matchId, Guid playerId)
    {
        MatchId = matchId;
        PlayerId = playerId;
        Goals = 0;
        Assists = 0;
        YellowCards = 0;
        RedCards = 0;
        MinutesPlayed = 0;
        Shots = 0;
        ShotsOnTarget = 0;
        Passes = 0;
        PassesCompleted = 0;
        Fouls = 0;
        FoulsReceived = 0;
    }

    public void UpdateStatistics(
        int goals = 0,
        int assists = 0,
        int yellowCards = 0,
        int redCards = 0,
        int minutesPlayed = 0,
        int shots = 0,
        int shotsOnTarget = 0,
        int passes = 0,
        int passesCompleted = 0,
        int fouls = 0,
        int foulsReceived = 0)
    {
        Goals = goals >= 0 ? goals : throw new ArgumentException("Goals cannot be negative", nameof(goals));
        Assists = assists >= 0 ? assists : throw new ArgumentException("Assists cannot be negative", nameof(assists));
        YellowCards = yellowCards >= 0 ? yellowCards : throw new ArgumentException("Yellow cards cannot be negative", nameof(yellowCards));
        RedCards = redCards >= 0 ? redCards : throw new ArgumentException("Red cards cannot be negative", nameof(redCards));
        MinutesPlayed = minutesPlayed >= 0 ? minutesPlayed : throw new ArgumentException("Minutes played cannot be negative", nameof(minutesPlayed));
        Shots = shots >= 0 ? shots : throw new ArgumentException("Shots cannot be negative", nameof(shots));
        ShotsOnTarget = shotsOnTarget >= 0 ? shotsOnTarget : throw new ArgumentException("Shots on target cannot be negative", nameof(shotsOnTarget));
        Passes = passes >= 0 ? passes : throw new ArgumentException("Passes cannot be negative", nameof(passes));
        PassesCompleted = passesCompleted >= 0 ? passesCompleted : throw new ArgumentException("Passes completed cannot be negative", nameof(passesCompleted));
        Fouls = fouls >= 0 ? fouls : throw new ArgumentException("Fouls cannot be negative", nameof(fouls));
        FoulsReceived = foulsReceived >= 0 ? foulsReceived : throw new ArgumentException("Fouls received cannot be negative", nameof(foulsReceived));
        
        SetUpdatedAt();
    }

    public void AddGoal()
    {
        Goals++;
        SetUpdatedAt();
    }

    public void AddAssist()
    {
        Assists++;
        SetUpdatedAt();
    }

    public void AddYellowCard()
    {
        YellowCards++;
        SetUpdatedAt();
    }

    public void AddRedCard()
    {
        RedCards++;
        SetUpdatedAt();
    }

    public double GetPassAccuracy()
    {
        if (Passes == 0) return 0;
        return (double)PassesCompleted / Passes * 100;
    }

    public double GetShotAccuracy()
    {
        if (Shots == 0) return 0;
        return (double)ShotsOnTarget / Shots * 100;
    }
}
