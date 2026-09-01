using MediatR;

namespace Sports.Application.Features.Teams.Commands.DeleteTeam;

public record DeleteTeamCommand(Guid Id) : IRequest<bool>;
