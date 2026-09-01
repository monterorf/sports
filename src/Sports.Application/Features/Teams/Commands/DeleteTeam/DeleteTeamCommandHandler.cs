using MediatR;
using Sports.Domain.Interfaces;

namespace Sports.Application.Features.Teams.Commands.DeleteTeam;

public class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTeamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _unitOfWork.Teams.GetByIdAsync(request.Id, cancellationToken);

        if (team == null)
            return false;

        team.Deactivate();

        await _unitOfWork.Teams.UpdateAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
