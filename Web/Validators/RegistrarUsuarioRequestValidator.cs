using FluentValidation;
using API.DTOs.Auth;
using Dominio.Validadores;

namespace API.Validators;

/// <summary>
/// Validador para las solicitudes de registro de usuario
/// </summary>
public class RegistrarUsuarioRequestValidator : AbstractValidator<RegistrarUsuarioRequest>
{
    public RegistrarUsuarioRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es requerido")
            .EmailAddress()
            .WithMessage("El formato del email no es válido")
            .MaximumLength(255)
            .WithMessage("El email no puede exceder 255 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es requerida")
            .MinimumLength(8)
            .WithMessage("La contraseña debe tener al menos 8 caracteres")
            .MaximumLength(100)
            .WithMessage("La contraseña no puede exceder 100 caracteres");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es requerido")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Apellido)
            .NotEmpty()
            .WithMessage("El apellido es requerido")
            .MaximumLength(100)
            .WithMessage("El apellido no puede exceder 100 caracteres");

        RuleFor(x => x.DNI)
            .GreaterThan(0)
            .WithMessage("El DNI debe ser un número válido");

        RuleFor(x => x.CUIL)
            .NotEmpty()
            .WithMessage("El CUIL es requerido")
            .Must(cuil => ValidadorCUIL.EsValido(cuil))
            .WithMessage("El CUIL no tiene un formato válido");

        RuleFor(x => x.Matricula)
            .NotEmpty()
            .WithMessage("La matrícula es requerida")
            .MaximumLength(50)
            .WithMessage("La matrícula no puede exceder 50 caracteres");

        RuleFor(x => x.TipoAutoridad)
            .IsInEnum()
            .WithMessage("El tipo de autoridad no es válido");
    }
}
