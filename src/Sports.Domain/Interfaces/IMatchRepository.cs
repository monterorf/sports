using Sports.Domain.Entities;

namespace Sports.Domain.Interfaces;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetByStatusAsync(Enums.MatchStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetByCompetitionAsync(string competition, CancellationToken cancellationToken = default);
    Task<Match> AddAsync(Match match, CancellationToken cancellationToken = default);
    Task UpdateAsync(Match match, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
