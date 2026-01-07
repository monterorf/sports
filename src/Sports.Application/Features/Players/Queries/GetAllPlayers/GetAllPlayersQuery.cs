using MediatR;
using Sports.Application.DTOs;

namespace Sports.Application.Features.Players.Queries.GetAllPlayers;

public record GetAllPlayersQuery : IRequest<IEnumerable<PlayerDto>>;
