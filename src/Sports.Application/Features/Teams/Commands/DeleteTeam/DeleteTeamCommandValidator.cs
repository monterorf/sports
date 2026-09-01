using FluentValidation;

namespace Sports.Application.Features.Teams.Commands.DeleteTeam;

public class DeleteTeamCommandValidator : AbstractValidator<DeleteTeamCommand>
{
    public DeleteTeamCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Team Id is required");
    }
}
