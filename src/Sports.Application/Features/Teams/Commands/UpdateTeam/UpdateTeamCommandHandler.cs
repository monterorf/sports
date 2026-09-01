using MediatR;
using Sports.Domain.Interfaces;

namespace Sports.Application.Features.Teams.Commands.UpdateTeam;

public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTeamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _unitOfWork.Teams.GetByIdAsync(request.Id, cancellationToken);

        if (team == null)
            return false;

        team.UpdateTeamInfo(
            request.Name,
            request.ShortName,
            request.City,
            request.Country,
            request.Logo,
            request.Stadium
        );

        await _unitOfWork.Teams.UpdateAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
