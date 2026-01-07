using Microsoft.EntityFrameworkCore;
using Sports.Domain.Entities;
using Sports.Domain.Enums;
using Sports.Domain.Interfaces;
using Sports.Infrastructure.Data;

namespace Sports.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly SportsDbContext _context;

    public PlayerRepository(SportsDbContext context)
    {
        _context = context;
    }

    public async Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .Include(p => p.Team)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Player>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .Include(p => p.Team)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Player>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .Include(p => p.Team)
            .Where(p => p.TeamId == teamId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Player>> GetByNationalityAsync(string nationality, CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .Include(p => p.Team)
            .Where(p => p.Nationality == nationality)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Player>> GetByPositionAsync(Position position, CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .Include(p => p.Team)
            .Where(p => p.Position == position)
            .ToListAsync(cancellationToken);
    }

    public async Task<Player> AddAsync(Player player, CancellationToken cancellationToken = default)
    {
        await _context.Players.AddAsync(player, cancellationToken);
        return player;
    }

    public Task UpdateAsync(Player player, CancellationToken cancellationToken = default)
    {
        _context.Players.Update(player);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var player = await _context.Players.FindAsync(new object[] { id }, cancellationToken);
        if (player != null)
        {
            _context.Players.Remove(player);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Players.AnyAsync(p => p.Id == id, cancellationToken);
    }
}
