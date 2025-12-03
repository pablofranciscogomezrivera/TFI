using FluentValidation;
using Web.DTOs.Urgencias;

namespace API.Validators;

/// <summary>
/// Validador para las solicitudes de registro de urgencias
/// </summary>
public class RegistrarUrgenciaRequestValidator : AbstractValidator<RegistrarUrgenciaRequest>
{
    public RegistrarUrgenciaRequestValidator()
    {
        // Validaciones para datos obligatorios
        RuleFor(x => x.CuilPaciente)
            .NotEmpty()
            .WithMessage("El CUIL del paciente es requerido")
            .Must(BeValidCuil)
            .WithMessage("El CUIL debe tener formato válido (11 dígitos, con o sin guiones)");

        RuleFor(x => x.Informe)
            .NotEmpty()
            .WithMessage("El informe de ingreso es requerido")
            .MinimumLength(5)
            .WithMessage("El informe debe tener al menos 5 caracteres")
            .MaximumLength(2000)
            .WithMessage("El informe no puede exceder 2000 caracteres");

        RuleFor(x => x.Temperatura)
            .InclusiveBetween(30, 45)
            .WithMessage("La temperatura debe estar entre 30°C y 45°C");

        RuleFor(x => x.FrecuenciaCardiaca)
            .InclusiveBetween(20, 250)
            .WithMessage("La frecuencia cardíaca debe estar entre 20 y 250 ppm");

        RuleFor(x => x.FrecuenciaRespiratoria)
            .InclusiveBetween(5, 60)
            .WithMessage("La frecuencia respiratoria debe estar entre 5 y 60 rpm");

        RuleFor(x => x.FrecuenciaSistolica)
            .InclusiveBetween(50, 250)
            .WithMessage("La presión sistólica debe estar entre 50 y 250 mmHg");

        RuleFor(x => x.FrecuenciaDiastolica)
            .InclusiveBetween(30, 180)
            .WithMessage("La presión diastólica debe estar entre 30 y 180 mmHg");

        RuleFor(x => x.NivelEmergencia)
            .IsInEnum()
            .WithMessage("Nivel de emergencia inválido");

        // Validaciones condicionales para datos opcionales del paciente
        When(x => !string.IsNullOrWhiteSpace(x.EmailPaciente), () =>
        {
            RuleFor(x => x.EmailPaciente)
                .EmailAddress()
                .WithMessage("El email del paciente no es válido");
        });

        When(x => x.TelefonoPaciente.HasValue, () =>
        {
            RuleFor(x => x.TelefonoPaciente)
                .GreaterThan(0)
                .WithMessage("El teléfono debe ser un número positivo");
        });

        When(x => x.FechaNacimientoPaciente.HasValue, () =>
        {
            RuleFor(x => x.FechaNacimientoPaciente)
                .LessThan(DateTime.Now)
                .WithMessage("La fecha de nacimiento debe ser anterior a hoy")
                .GreaterThan(DateTime.Now.AddYears(-120))
                .WithMessage("La fecha de nacimiento no puede ser mayor a 120 años");
        });

        When(x => !string.IsNullOrWhiteSpace(x.NombrePaciente), () =>
        {
            RuleFor(x => x.NombrePaciente)
                .MinimumLength(2)
                .WithMessage("El nombre debe tener al menos 2 caracteres")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder 100 caracteres");
        });

        When(x => !string.IsNullOrWhiteSpace(x.ApellidoPaciente), () =>
        {
            RuleFor(x => x.ApellidoPaciente)
                .MinimumLength(2)
                .WithMessage("El apellido debe tener al menos 2 caracteres")
                .MaximumLength(100)
                .WithMessage("El apellido no puede exceder 100 caracteres");
        });
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
