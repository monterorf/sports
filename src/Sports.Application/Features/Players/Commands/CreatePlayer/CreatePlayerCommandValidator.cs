using FluentValidation;

namespace Sports.Application.Features.Players.Commands.CreatePlayer;

public class CreatePlayerCommandValidator : AbstractValidator<CreatePlayerCommand>
{
    public CreatePlayerCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.Now.AddYears(-100)).WithMessage("Date of birth is not valid");

        RuleFor(x => x.Nationality)
            .NotEmpty().WithMessage("Nationality is required")
            .MaximumLength(50).WithMessage("Nationality must not exceed 50 characters");

        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("Height must be greater than 0")
            .LessThan(300).WithMessage("Height must be less than 300 cm");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0")
            .LessThan(300).WithMessage("Weight must be less than 300 kg");

        RuleFor(x => x.JerseyNumber)
            .GreaterThan(0).WithMessage("Jersey number must be greater than 0")
            .LessThanOrEqualTo(99).WithMessage("Jersey number must be less than or equal to 99");

        RuleFor(x => x.Position)
            .IsInEnum().WithMessage("Invalid position");

        RuleFor(x => x.TeamId)
            .NotEmpty().WithMessage("Team ID is required");
    }
}
