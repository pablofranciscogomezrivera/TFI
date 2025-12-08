using FluentValidation;
using API.DTOs.Atenciones;

namespace API.Validators;

/// <summary>
/// Validador para las solicitudes de registro de atención médica
/// </summary>
public class RegistrarAtencionRequestValidator : AbstractValidator<RegistrarAtencionRequest>
{
    public RegistrarAtencionRequestValidator()
    {
        RuleFor(x => x.CuilPaciente)
            .NotEmpty()
            .WithMessage("El CUIL del paciente es requerido")
            .Must(BeValidCuil)
            .WithMessage("El CUIL debe tener formato válido (11 dígitos, con o sin guiones)");

        RuleFor(x => x.InformeMedico)
            .NotEmpty()
            .WithMessage("El informe médico es requerido")
            .MinimumLength(10)
            .WithMessage("El informe médico debe tener al menos 10 caracteres")
            .MaximumLength(5000)
            .WithMessage("El informe médico no puede exceder 5000 caracteres");
    }

    private bool BeValidCuil(string? cuil)
    {
        if (string.IsNullOrWhiteSpace(cuil))
            return false;

        // Remover guiones si existen
        var cuilLimpio = cuil.Replace("-", "").Replace(" ", "");

        // Debe tener exactamente 11 dígitos
        if (cuilLimpio.Length != 11)
            return false;

        // Debe contener solo números
        return cuilLimpio.All(char.IsDigit);
    }
}
