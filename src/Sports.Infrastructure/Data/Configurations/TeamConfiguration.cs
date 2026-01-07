using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sports.Domain.Entities;

namespace Sports.Infrastructure.Data.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.ShortName)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(t => t.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Logo)
            .HasMaxLength(500);

        builder.Property(t => t.Stadium)
            .HasMaxLength(200);

        builder.Property(t => t.SportType)
            .IsRequired()
            .HasConversion<string>();

        builder.HasMany(t => t.Players)
            .WithOne(p => p.Team)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.HomeMatches)
            .WithOne(m => m.HomeTeam)
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.AwayMatches)
            .WithOne(m => m.AwayTeam)
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.Name);
        builder.HasIndex(t => t.ShortName);
    }
}
