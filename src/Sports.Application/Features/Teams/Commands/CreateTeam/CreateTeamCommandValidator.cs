using FluentValidation;

namespace Sports.Application.Features.Teams.Commands.CreateTeam;

public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required")
            .MaximumLength(100).WithMessage("Team name must not exceed 100 characters");

        RuleFor(x => x.ShortName)
            .NotEmpty().WithMessage("Short name is required")
            .MaximumLength(10).WithMessage("Short name must not exceed 10 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters");

        RuleFor(x => x.FoundedDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Founded date cannot be in the future");

        RuleFor(x => x.SportType)
            .IsInEnum().WithMessage("Invalid sport type");
    }
}
