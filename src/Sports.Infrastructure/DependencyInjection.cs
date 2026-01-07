using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sports.Domain.Interfaces;
using Sports.Infrastructure.Data;
using Sports.Infrastructure.Repositories;

namespace Sports.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SportsDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(SportsDbContext).Assembly.FullName)
                      .EnableRetryOnFailure(
                          maxRetryCount: 3,
                          maxRetryDelay: TimeSpan.FromSeconds(5),
                          errorNumbersToAdd: null)));

        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IMatchStatisticRepository, MatchStatisticRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
