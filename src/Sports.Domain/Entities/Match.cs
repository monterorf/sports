using Sports.Domain.Common;
using Sports.Domain.Enums;

namespace Sports.Domain.Entities;

public class Match : BaseEntity
{
    public Guid HomeTeamId { get; private set; }
    public Team HomeTeam { get; private set; }

    public Guid AwayTeamId { get; private set; }
    public Team AwayTeam { get; private set; }

    public DateTime ScheduledDate { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    
    public string Venue { get; private set; }
    public string? Competition { get; private set; }
    public string? Season { get; private set; }
    
    public int HomeTeamScore { get; private set; }
    public int AwayTeamScore { get; private set; }
    
    public MatchStatus Status { get; private set; }
    public SportType SportType { get; private set; }

    private readonly List<MatchStatistic> _statistics = new();
    public IReadOnlyCollection<MatchStatistic> Statistics => _statistics.AsReadOnly();

    private Match() { }

    public Match(
        Guid homeTeamId,
        Guid awayTeamId,
        DateTime scheduledDate,
        string venue,
        SportType sportType,
        string? competition = null,
        string? season = null)
    {
        if (homeTeamId == awayTeamId)
            throw new ArgumentException("Home team and away team cannot be the same");

        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        ScheduledDate = scheduledDate;
        Venue = venue ?? throw new ArgumentNullException(nameof(venue));
        SportType = sportType;
        Competition = competition;
        Season = season;
        Status = MatchStatus.Scheduled;
        HomeTeamScore = 0;
        AwayTeamScore = 0;
    }

    public void StartMatch()
    {
        if (Status != MatchStatus.Scheduled)
            throw new InvalidOperationException("Match can only be started from scheduled status");

        Status = MatchStatus.InProgress;
        StartDate = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void FinishMatch()
    {
        if (Status != MatchStatus.InProgress)
            throw new InvalidOperationException("Match must be in progress to finish");

        Status = MatchStatus.Finished;
        EndDate = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void PostponeMatch(DateTime newScheduledDate)
    {
        if (Status == MatchStatus.Finished)
            throw new InvalidOperationException("Cannot postpone a finished match");

        Status = MatchStatus.Postponed;
        ScheduledDate = newScheduledDate;
        SetUpdatedAt();
    }

    public void CancelMatch()
    {
        if (Status == MatchStatus.Finished)
            throw new InvalidOperationException("Cannot cancel a finished match");

        Status = MatchStatus.Cancelled;
        SetUpdatedAt();
    }

    public void SuspendMatch()
    {
        if (Status != MatchStatus.InProgress)
            throw new InvalidOperationException("Can only suspend a match in progress");

        Status = MatchStatus.Suspended;
        SetUpdatedAt();
    }

    public void ResumeMatch()
    {
        if (Status != MatchStatus.Suspended)
            throw new InvalidOperationException("Can only resume a suspended match");

        Status = MatchStatus.InProgress;
        SetUpdatedAt();
    }

    public void UpdateScore(int homeScore, int awayScore)
    {
        if (Status != MatchStatus.InProgress && Status != MatchStatus.Finished)
            throw new InvalidOperationException("Can only update score during or after the match");

        if (homeScore < 0 || awayScore < 0)
            throw new ArgumentException("Scores cannot be negative");

        HomeTeamScore = homeScore;
        AwayTeamScore = awayScore;
        SetUpdatedAt();
    }

    public void AddStatistic(MatchStatistic statistic)
    {
        if (statistic == null)
            throw new ArgumentNullException(nameof(statistic));

        _statistics.Add(statistic);
        SetUpdatedAt();
    }

    public string GetWinner()
    {
        if (Status != MatchStatus.Finished)
            return "Match not finished";

        if (HomeTeamScore > AwayTeamScore)
            return "Home Team";
        else if (AwayTeamScore > HomeTeamScore)
            return "Away Team";
        else
            return "Draw";
    }
}
