using MediatR;
using Sports.Domain.Enums;

namespace Sports.Application.Features.Teams.Commands.CreateTeam;

public record CreateTeamCommand(
    string Name,
    string ShortName,
    DateTime FoundedDate,
    string City,
    string Country,
    SportType SportType,
    string? Logo = null,
    string? Stadium = null
) : IRequest<Guid>;
