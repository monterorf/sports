using MediatR;
using Sports.Domain.Entities;
using Sports.Domain.Interfaces;

namespace Sports.Application.Features.Players.Commands.CreatePlayer;

public class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePlayerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
    {
        var teamExists = await _unitOfWork.Teams.ExistsAsync(request.TeamId, cancellationToken);
        if (!teamExists)
            throw new InvalidOperationException($"Team with ID {request.TeamId} does not exist");

        var player = new Player(
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Nationality,
            request.Height,
            request.Weight,
            request.Position,
            request.JerseyNumber,
            request.TeamId,
            request.Photo
        );

        await _unitOfWork.Players.AddAsync(player, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return player.Id;
    }
}
