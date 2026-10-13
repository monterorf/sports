using MediatR;
using Sports.Application.DTOs;

namespace Sports.Application.Features.Players.Queries.GetPlayerById
{
    public record GetPlayerByIdQuery(Guid Id) : IRequest<PlayerDto?>;
}
