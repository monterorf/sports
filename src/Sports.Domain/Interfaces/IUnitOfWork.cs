namespace Sports.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ITeamRepository Teams { get; }
    IPlayerRepository Players { get; }
    IMatchRepository Matches { get; }
    IMatchStatisticRepository MatchStatistics { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
