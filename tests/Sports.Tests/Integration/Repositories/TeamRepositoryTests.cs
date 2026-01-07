using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sports.Domain.Entities;
using Sports.Domain.Enums;
using Sports.Infrastructure.Data;
using Sports.Infrastructure.Repositories;
using Xunit;

namespace Sports.Tests.Integration.Repositories;

public class TeamRepositoryTests : IDisposable
{
    private readonly SportsDbContext _context;
    private readonly TeamRepository _repository;

    public TeamRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SportsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new SportsDbContext(options);
        _repository = new TeamRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTeam_WhenTeamExists()
    {
        // Arrange
        var team = new Team(
            name: "Test Team",
            shortName: "TT",
            foundedDate: new DateTime(2020, 1, 1),
            city: "Test City",
            country: "Test Country",
            sportType: SportType.Football
        );
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(team.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(team.Id);
        result.Name.Should().Be(team.Name);
        result.City.Should().Be(team.City);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTeamDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTeams()
    {
        // Arrange
        var teams = new List<Team>
        {
            new Team(
                name: "Team 1",
                shortName: "T1",
                foundedDate: new DateTime(2020, 1, 1),
                city: "City 1",
                country: "Country 1",
                sportType: SportType.Football
            ),
            new Team(
                name: "Team 2",
                shortName: "T2",
                foundedDate: new DateTime(2021, 1, 1),
                city: "City 2",
                country: "Country 2",
                sportType: SportType.Basketball
            )
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Name == "Team 1");
        result.Should().Contain(t => t.Name == "Team 2");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoTeamsExist()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByCountryAsync_ShouldReturnTeamsFromSpecifiedCountry()
    {
        // Arrange
        var teams = new List<Team>
        {
            new Team(
                name: "Team USA",
                shortName: "TUSA",
                foundedDate: new DateTime(2020, 1, 1),
                city: "New York",
                country: "USA",
                sportType: SportType.Football
            ),
            new Team(
                name: "Team Spain",
                shortName: "TS",
                foundedDate: new DateTime(2021, 1, 1),
                city: "Madrid",
                country: "Spain",
                sportType: SportType.Football
            ),
            new Team(
                name: "Team USA 2",
                shortName: "TUSA2",
                foundedDate: new DateTime(2022, 1, 1),
                city: "Los Angeles",
                country: "USA",
                sportType: SportType.Basketball
            )
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCountryAsync("USA");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.Country == "USA");
    }

    [Fact]
    public async Task GetBySportTypeAsync_ShouldReturnTeamsOfSpecifiedSport()
    {
        // Arrange
        var teams = new List<Team>
        {
            new Team(
                name: "Football Team 1",
                shortName: "FT1",
                foundedDate: new DateTime(2020, 1, 1),
                city: "City 1",
                country: "Country 1",
                sportType: SportType.Football
            ),
            new Team(
                name: "Basketball Team",
                shortName: "BT",
                foundedDate: new DateTime(2021, 1, 1),
                city: "City 2",
                country: "Country 2",
                sportType: SportType.Basketball
            ),
            new Team(
                name: "Football Team 2",
                shortName: "FT2",
                foundedDate: new DateTime(2022, 1, 1),
                city: "City 3",
                country: "Country 3",
                sportType: SportType.Football
            )
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySportTypeAsync(SportType.Football);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.SportType == SportType.Football);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnTeam_WhenTeamExists()
    {
        // Arrange
        var team = new Team(
            name: "Unique Team Name",
            shortName: "UTN",
            foundedDate: new DateTime(2020, 1, 1),
            city: "Test City",
            country: "Test Country",
            sportType: SportType.Football
        );
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("Unique Team Name");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Unique Team Name");
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenTeamDoesNotExist()
    {
        // Act
        var result = await _repository.GetByNameAsync("Non-existent Team");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldAddTeamToDatabase()
    {
        // Arrange
        var team = new Team(
            name: "New Team",
            shortName: "NT",
            foundedDate: new DateTime(2023, 1, 1),
            city: "New City",
            country: "New Country",
            sportType: SportType.Football
        );

        // Act
        var result = await _repository.AddAsync(team);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(team.Id);

        var savedTeam = await _context.Teams.FindAsync(team.Id);
        savedTeam.Should().NotBeNull();
        savedTeam!.Name.Should().Be("New Team");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTeamInDatabase()
    {
        // Arrange
        var team = new Team(
            name: "Original Name",
            shortName: "ON",
            foundedDate: new DateTime(2020, 1, 1),
            city: "Original City",
            country: "Original Country",
            sportType: SportType.Football
        );
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();

        // Act
        team.UpdateTeamInfo("Updated Name", "UN", "Updated City", "Updated Country");
        await _repository.UpdateAsync(team);
        await _context.SaveChangesAsync();

        // Assert
        var updatedTeam = await _context.Teams.FindAsync(team.Id);
        updatedTeam.Should().NotBeNull();
        updatedTeam!.Name.Should().Be("Updated Name");
        updatedTeam.City.Should().Be("Updated City");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTeamFromDatabase()
    {
        // Arrange
        var team = new Team(
            name: "Team to Delete",
            shortName: "TTD",
            foundedDate: new DateTime(2020, 1, 1),
            city: "Test City",
            country: "Test Country",
            sportType: SportType.Football
        );
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(team.Id);
        await _context.SaveChangesAsync();

        // Assert
        var deletedTeam = await _context.Teams.FindAsync(team.Id);
        deletedTeam.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDoNothing_WhenTeamDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var act = async () => await _repository.DeleteAsync(nonExistentId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenTeamExists()
    {
        // Arrange
        var team = new Team(
            name: "Existing Team",
            shortName: "ET",
            foundedDate: new DateTime(2020, 1, 1),
            city: "Test City",
            country: "Test Country",
            sportType: SportType.Football
        );
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(team.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenTeamDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.ExistsAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludePlayers()
    {
        // Arrange
        var team = new Team(
            name: "Team with Players",
            shortName: "TWP",
            foundedDate: new DateTime(2020, 1, 1),
            city: "Test City",
            country: "Test Country",
            sportType: SportType.Football
        );
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();

        var player = new Player(
            firstName: "John",
            lastName: "Doe",
            dateOfBirth: new DateTime(1990, 1, 1),
            nationality: "USA",
            height: 180,
            weight: 75,
            position: Position.Forward,
            jerseyNumber: 10,
            teamId: team.Id
        );
        await _context.Players.AddAsync(player);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetByIdAsync(team.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Players.Should().NotBeNull();
        result.Players.Should().HaveCount(1);
        result.Players.First().FirstName.Should().Be("John");
    }
}
