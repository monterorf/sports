using Sports.Domain.Entities;

namespace Sports.Domain.Interfaces;

public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Player>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Player>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Player>> GetByNationalityAsync(string nationality, CancellationToken cancellationToken = default);
    Task<IEnumerable<Player>> GetByPositionAsync(Enums.Position position, CancellationToken cancellationToken = default);
    Task<Player> AddAsync(Player player, CancellationToken cancellationToken = default);
    Task UpdateAsync(Player player, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
