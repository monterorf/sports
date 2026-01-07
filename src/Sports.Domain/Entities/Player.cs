using Sports.Domain.Common;
using Sports.Domain.Enums;

namespace Sports.Domain.Entities;

public class Player : BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string FullName => $"{FirstName} {LastName}";
    public DateTime DateOfBirth { get; private set; }
    public string Nationality { get; private set; }
    public int Height { get; private set; }
    public int Weight { get; private set; }
    public Position Position { get; private set; }
    public int JerseyNumber { get; private set; }
    public string? Photo { get; private set; }
    public bool IsActive { get; private set; }

    public Guid TeamId { get; private set; }
    public Team Team { get; private set; }

    private readonly List<MatchStatistic> _statistics = new();
    public IReadOnlyCollection<MatchStatistic> Statistics => _statistics.AsReadOnly();

    private Player() { }

    public Player(
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        string nationality,
        int height,
        int weight,
        Position position,
        int jerseyNumber,
        Guid teamId,
        string? photo = null)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        DateOfBirth = dateOfBirth;
        Nationality = nationality ?? throw new ArgumentNullException(nameof(nationality));
        Height = height > 0 ? height : throw new ArgumentException("Height must be greater than 0", nameof(height));
        Weight = weight > 0 ? weight : throw new ArgumentException("Weight must be greater than 0", nameof(weight));
        Position = position;
        JerseyNumber = jerseyNumber;
        TeamId = teamId;
        Photo = photo;
        IsActive = true;
    }

    public void UpdatePlayerInfo(
        string firstName,
        string lastName,
        string nationality,
        int height,
        int weight,
        Position position,
        int jerseyNumber,
        string? photo = null)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Nationality = nationality ?? throw new ArgumentNullException(nameof(nationality));
        Height = height > 0 ? height : throw new ArgumentException("Height must be greater than 0", nameof(height));
        Weight = weight > 0 ? weight : throw new ArgumentException("Weight must be greater than 0", nameof(weight));
        Position = position;
        JerseyNumber = jerseyNumber;
        Photo = photo;
        SetUpdatedAt();
    }

    public void TransferToTeam(Guid newTeamId)
    {
        TeamId = newTeamId;
        SetUpdatedAt();
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

    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}
