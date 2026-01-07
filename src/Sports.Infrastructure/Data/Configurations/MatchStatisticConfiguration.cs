using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sports.Domain.Entities;

namespace Sports.Infrastructure.Data.Configurations;

public class MatchStatisticConfiguration : IEntityTypeConfiguration<MatchStatistic>
{
    public void Configure(EntityTypeBuilder<MatchStatistic> builder)
    {
        builder.HasKey(ms => ms.Id);

        builder.HasOne(ms => ms.Match)
            .WithMany(m => m.Statistics)
            .HasForeignKey(ms => ms.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ms => ms.Player)
            .WithMany(p => p.Statistics)
            .HasForeignKey(ms => ms.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ms => new { ms.MatchId, ms.PlayerId })
            .IsUnique();
    }
}
