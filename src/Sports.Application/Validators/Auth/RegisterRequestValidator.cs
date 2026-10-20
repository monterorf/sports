using FluentValidation;
using Sports.Application.DTOs.Auth;

namespace Sports.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email es requerido")
            .EmailAddress().WithMessage("Email debe ser válido");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName es requerido")
            .MinimumLength(2).WithMessage("FirstName debe tener mínimo 2 caracteres");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName es requerido")
            .MinimumLength(2).WithMessage("LastName debe tener mínimo 2 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password es requerido")
            .MinimumLength(8).WithMessage("Password debe tener mínimo 8 caracteres")
            .Must(x => x.Any(char.IsUpper)).WithMessage("Password debe contener al menos una mayúscula")
            .Must(x => x.Any(char.IsLower)).WithMessage("Password debe contener al menos una minúscula")
            .Must(x => x.Any(char.IsDigit)).WithMessage("Password debe contener al menos un número")
            .Must(x => x.Any(c => !char.IsLetterOrDigit(c))).WithMessage("Password debe contener al menos un carácter especial");
    }
}
