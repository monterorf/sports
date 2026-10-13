using MediatR;
using Sports.Domain.Interfaces;
namespace Sports.Application.Features.Players.Commands.UpdatePlayer;

public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdatePlayerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var player = await _unitOfWork.Players.GetByIdAsync(request.Id, cancellationToken);
        if (player == null) return false;

        if (player.TeamId != request.TeamId)
        {
            var teamExists = await _unitOfWork.Teams.ExistsAsync(request.TeamId, cancellationToken);
            if (!teamExists)
                throw new InvalidOperationException($"Team with ID {request.TeamId} does not exist");

            player.TransferToTeam(request.TeamId);
        }

        player.UpdatePlayerInfo(
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Nationality,
            request.Height,
            request.Weight,
            request.Position,
            request.JerseyNumber,
            request.Photo
        );

        await _unitOfWork.Players.UpdateAsync(player, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
