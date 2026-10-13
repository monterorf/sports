using MediatR;
namespace Sports.Application.Features.Players.Commands.DeletePlayer;

public record DeletePlayerCommand(Guid Id) : IRequest<bool>;

