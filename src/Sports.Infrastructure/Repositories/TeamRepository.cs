using Microsoft.EntityFrameworkCore;
using Sports.Domain.Entities;
using Sports.Domain.Enums;
using Sports.Domain.Interfaces;
using Sports.Infrastructure.Data;

namespace Sports.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly SportsDbContext _context;

    public TeamRepository(SportsDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Team>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Team>> GetByCountryAsync(string country, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .Where(t => t.Country == country)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Team>> GetBySportTypeAsync(SportType sportType, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .Where(t => t.SportType == sportType)
            .ToListAsync(cancellationToken);
    }

    public async Task<Team?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<Team> AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        await _context.Teams.AddAsync(team, cancellationToken);
        return team;
    }

    public Task UpdateAsync(Team team, CancellationToken cancellationToken = default)
    {
        _context.Teams.Update(team);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var team = await _context.Teams.FindAsync(new object[] { id }, cancellationToken);
        if (team != null)
        {
            _context.Teams.Remove(team);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Teams.AnyAsync(t => t.Id == id, cancellationToken);
    }
}
