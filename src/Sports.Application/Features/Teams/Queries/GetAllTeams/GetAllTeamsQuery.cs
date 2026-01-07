using MediatR;
using Sports.Application.DTOs;

namespace Sports.Application.Features.Teams.Queries.GetAllTeams;

public record GetAllTeamsQuery : IRequest<IEnumerable<TeamDto>>;
