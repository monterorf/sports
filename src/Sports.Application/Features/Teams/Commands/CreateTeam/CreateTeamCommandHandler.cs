using MediatR;
using Sports.Domain.Entities;
using Sports.Domain.Interfaces;

namespace Sports.Application.Features.Teams.Commands.CreateTeam;

public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTeamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        var team = new Team(
            request.Name,
            request.ShortName,
            request.FoundedDate,
            request.City,
            request.Country,
            request.SportType,
            request.Logo,
            request.Stadium
        );

        await _unitOfWork.Teams.AddAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return team.Id;
    }
}
