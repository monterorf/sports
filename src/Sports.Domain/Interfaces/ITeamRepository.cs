using Sports.Domain.Entities;

namespace Sports.Domain.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Team>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Team>> GetByCountryAsync(string country, CancellationToken cancellationToken = default);
    Task<IEnumerable<Team>> GetBySportTypeAsync(Enums.SportType sportType, CancellationToken cancellationToken = default);
    Task<Team?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Team> AddAsync(Team team, CancellationToken cancellationToken = default);
    Task UpdateAsync(Team team, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
