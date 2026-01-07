using Microsoft.EntityFrameworkCore;
using Sports.Domain.Entities;
using Sports.Domain.Interfaces;
using Sports.Infrastructure.Data;

namespace Sports.Infrastructure.Repositories;

public class MatchStatisticRepository : IMatchStatisticRepository
{
    private readonly SportsDbContext _context;

    public MatchStatisticRepository(SportsDbContext context)
    {
        _context = context;
    }

    public async Task<MatchStatistic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MatchStatistics
            .Include(ms => ms.Match)
            .Include(ms => ms.Player)
            .FirstOrDefaultAsync(ms => ms.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MatchStatistic>> GetByMatchIdAsync(Guid matchId, CancellationToken cancellationToken = default)
    {
        return await _context.MatchStatistics
            .Include(ms => ms.Player)
            .Where(ms => ms.MatchId == matchId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MatchStatistic>> GetByPlayerIdAsync(Guid playerId, CancellationToken cancellationToken = default)
    {
        return await _context.MatchStatistics
            .Include(ms => ms.Match)
            .Where(ms => ms.PlayerId == playerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<MatchStatistic?> GetByMatchAndPlayerAsync(Guid matchId, Guid playerId, CancellationToken cancellationToken = default)
    {
        return await _context.MatchStatistics
            .Include(ms => ms.Match)
            .Include(ms => ms.Player)
            .FirstOrDefaultAsync(ms => ms.MatchId == matchId && ms.PlayerId == playerId, cancellationToken);
    }

    public async Task<MatchStatistic> AddAsync(MatchStatistic statistic, CancellationToken cancellationToken = default)
    {
        await _context.MatchStatistics.AddAsync(statistic, cancellationToken);
        return statistic;
    }

    public Task UpdateAsync(MatchStatistic statistic, CancellationToken cancellationToken = default)
    {
        _context.MatchStatistics.Update(statistic);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var statistic = await _context.MatchStatistics.FindAsync(new object[] { id }, cancellationToken);
        if (statistic != null)
        {
            _context.MatchStatistics.Remove(statistic);
        }
    }
}
