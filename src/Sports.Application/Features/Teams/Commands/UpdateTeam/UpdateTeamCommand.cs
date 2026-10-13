using MediatR;
using Sports.Domain.Enums;

namespace Sports.Application.Features.Teams.Commands.UpdateTeam;

public record UpdateTeamCommand(
    Guid Id,
    string Name,
    string ShortName,
    DateTime FoundedDate,
    string City,
    string Country,
    SportType SportType,
    string? Logo = null,
    string? Stadium = null,
    bool IsActive = true
) : IRequest<bool>;
