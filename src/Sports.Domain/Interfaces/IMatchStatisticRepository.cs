using Sports.Domain.Entities;

namespace Sports.Domain.Interfaces;

public interface IMatchStatisticRepository
{
    Task<MatchStatistic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MatchStatistic>> GetByMatchIdAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MatchStatistic>> GetByPlayerIdAsync(Guid playerId, CancellationToken cancellationToken = default);
    Task<MatchStatistic?> GetByMatchAndPlayerAsync(Guid matchId, Guid playerId, CancellationToken cancellationToken = default);
    Task<MatchStatistic> AddAsync(MatchStatistic statistic, CancellationToken cancellationToken = default);
    Task UpdateAsync(MatchStatistic statistic, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
