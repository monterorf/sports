using Microsoft.EntityFrameworkCore;
using Sports.Domain.Entities;
using Sports.Domain.Enums;
using Sports.Domain.Interfaces;
using Sports.Infrastructure.Data;

namespace Sports.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly SportsDbContext _context;

    public MatchRepository(SportsDbContext context)
    {
        _context = context;
    }

    public async Task<Match?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Statistics)
                .ThenInclude(s => s.Player)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Match>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Match>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Match>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.ScheduledDate >= startDate && m.ScheduledDate <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Match>> GetByStatusAsync(MatchStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Match>> GetByCompetitionAsync(string competition, CancellationToken cancellationToken = default)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.Competition == competition)
            .ToListAsync(cancellationToken);
    }

    public async Task<Match> AddAsync(Match match, CancellationToken cancellationToken = default)
    {
        await _context.Matches.AddAsync(match, cancellationToken);
        return match;
    }

    public Task UpdateAsync(Match match, CancellationToken cancellationToken = default)
    {
        _context.Matches.Update(match);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var match = await _context.Matches.FindAsync(new object[] { id }, cancellationToken);
        if (match != null)
        {
            _context.Matches.Remove(match);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Matches.AnyAsync(m => m.Id == id, cancellationToken);
    }
}
