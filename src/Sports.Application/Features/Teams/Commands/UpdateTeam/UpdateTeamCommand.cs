using MediatR;

namespace Sports.Application.Features.Teams.Commands.UpdateTeam;

public record UpdateTeamCommand(
    Guid Id,
    string Name,
    string ShortName,
    string City,
    string Country,
    string? Logo = null,
    string? Stadium = null
) : IRequest<bool>;
