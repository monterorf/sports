using MediatR;
using Sports.Application.DTOs;

namespace Sports.Application.Features.Teams.Queries.GetTeamById;

public record GetTeamByIdQuery(Guid Id) : IRequest<TeamDto?>;
