using MediatR;
using Sports.Application.DTOs;

namespace Sports.Application.Features.Matches.Queries.GetAllMatches;

public record GetAllMatchesQuery : IRequest<IEnumerable<MatchDto>>;
