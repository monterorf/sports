using FluentValidation;
using Sports.Application.DTOs.Auth;

namespace Sports.Application.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email es requerido")
            .EmailAddress().WithMessage("Email debe ser válido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password es requerido");
    }
}
