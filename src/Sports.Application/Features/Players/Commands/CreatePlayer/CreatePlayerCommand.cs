using MediatR;
using Sports.Domain.Enums;

namespace Sports.Application.Features.Players.Commands.CreatePlayer;

public record CreatePlayerCommand(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Nationality,
    int Height,
    int Weight,
    Position Position,
    int JerseyNumber,
    Guid TeamId,
    string? Photo = null
) : IRequest<Guid>;
