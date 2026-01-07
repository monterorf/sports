using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sports.Infrastructure.Data;

public class SportsDbContextFactory : IDesignTimeDbContextFactory<SportsDbContext>
{
    public SportsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SportsDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SportsDb;Trusted_Connection=true;MultipleActiveResultSets=true");

        return new SportsDbContext(optionsBuilder.Options);
    }
}
