using Dominio.Entidades;
using Dominio.Enums;

namespace API.DTOs.Urgencias;

public class RegistrarUrgenciaRequest
{
    // Datos obligatorios del ingreso
    public string CuilPaciente { get; set; } = string.Empty;
    public string Informe { get; set; } = string.Empty;
    public double Temperatura { get; set; }
    public NivelEmergencia NivelEmergencia { get; set; }
    public double FrecuenciaCardiaca { get; set; }
    public double FrecuenciaRespiratoria { get; set; }
    public double FrecuenciaSistolica { get; set; }
    public double FrecuenciaDiastolica { get; set; }

    // Datos opcionales del paciente (si no existe en el sistema)
    public string? NombrePaciente { get; set; }
    public string? ApellidoPaciente { get; set; }
    public string? CallePaciente { get; set; }
    public int? NumeroPaciente { get; set; }
    public string? LocalidadPaciente { get; set; }
    public string? EmailPaciente { get; set; }
    public long? TelefonoPaciente { get; set; }
    public int? ObraSocialIdPaciente { get; set; }
    public string? NumeroAfiliadoPaciente { get; set; }
    public DateTime? FechaNacimientoPaciente { get; set; }
}