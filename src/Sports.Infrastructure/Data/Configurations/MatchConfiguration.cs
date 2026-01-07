using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sports.Domain.Entities;

namespace Sports.Infrastructure.Data.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Venue)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Competition)
            .HasMaxLength(200);

        builder.Property(m => m.Season)
            .HasMaxLength(50);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.SportType)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(m => m.HomeTeam)
            .WithMany(t => t.HomeMatches)
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.AwayTeam)
            .WithMany(t => t.AwayMatches)
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.Statistics)
            .WithOne(s => s.Match)
            .HasForeignKey(s => s.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => m.ScheduledDate);
        builder.HasIndex(m => m.Status);
    }
}
