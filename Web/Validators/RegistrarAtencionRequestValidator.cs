using API.DTOs.Atenciones;
using Dominio.Validadores;
using FluentValidation;

namespace API.Validators;

/// <summary>
/// Validador para las solicitudes de registro de atención médica
/// </summary>
public class RegistrarAtencionRequestValidator : AbstractValidator<RegistrarAtencionRequest>
{
    public RegistrarAtencionRequestValidator()
    {
        RuleFor(x => x.InformeMedico)
            .NotEmpty()
            .WithMessage("El informe médico es requerido")
            .MinimumLength(10)
            .WithMessage("El informe médico debe tener al menos 10 caracteres")
            .MaximumLength(5000)
            .WithMessage("El informe médico no puede exceder 5000 caracteres");
    }
}
