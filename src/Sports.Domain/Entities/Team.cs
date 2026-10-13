using Sports.Domain.Common;
using Sports.Domain.Enums;

namespace Sports.Domain.Entities;

public class Team : BaseEntity
{
    public string Name { get; private set; }
    public string ShortName { get; private set; }
    public string? Logo { get; private set; }
    public DateTime FoundedDate { get; private set; }
    public string City { get; private set; }
    public string Country { get; private set; }
    public string? Stadium { get; private set; }
    public SportType SportType { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Player> _players = new();
    public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

    private readonly List<Match> _homeMatches = new();
    public IReadOnlyCollection<Match> HomeMatches => _homeMatches.AsReadOnly();

    private readonly List<Match> _awayMatches = new();
    public IReadOnlyCollection<Match> AwayMatches => _awayMatches.AsReadOnly();

    private Team() { }

    public Team(
        string name,
        string shortName,
        DateTime foundedDate,
        string city,
        string country,
        SportType sportType,
        string? logo = null,
        string? stadium = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ShortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
        FoundedDate = foundedDate;
        City = city ?? throw new ArgumentNullException(nameof(city));
        Country = country ?? throw new ArgumentNullException(nameof(country));
        SportType = sportType;
        Logo = logo;
        Stadium = stadium;
        IsActive = true;
    }

    public void UpdateTeamInfo(string name, string shortName,DateTime foundedDate, string city, string country, string? logo = null, string? stadium = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ShortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
        if (foundedDate == default(DateTime))
            throw new ArgumentNullException(nameof(foundedDate));
        FoundedDate = foundedDate; 
        City = city ?? throw new ArgumentNullException(nameof(city));
        Country = country ?? throw new ArgumentNullException(nameof(country));
        Logo = logo;
        Stadium = stadium;
        SetUpdatedAt();
    }

    public void AddPlayer(Player player)
    {
        if (player == null)
            throw new ArgumentNullException(nameof(player));

        if (_players.Any(p => p.Id == player.Id))
            throw new InvalidOperationException("Player already exists in the team");

        _players.Add(player);
        SetUpdatedAt();
    }

    public void RemovePlayer(Guid playerId)
    {
        var player = _players.FirstOrDefault(p => p.Id == playerId);
        if (player != null)
        {
            _players.Remove(player);
            SetUpdatedAt();
        }
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }
}
