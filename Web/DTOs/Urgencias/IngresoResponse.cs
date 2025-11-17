using Dominio.Entidades;

namespace Web.DTOs.Urgencias;

public class IngresoResponse
{
    public string CuilPaciente { get; set; } = string.Empty;
    public string NombrePaciente { get; set; } = string.Empty;
    public string ApellidoPaciente { get; set; } = string.Empty;
    public string InformeInicial { get; set; } = string.Empty;
    public NivelEmergencia NivelEmergencia { get; set; }
    public EstadoIngreso Estado { get; set; }
    public DateTime FechaIngreso { get; set; }
    public SignosVitalesDto SignosVitales { get; set; } = new();
}

public class SignosVitalesDto
{
    public double Temperatura { get; set; }
    public double FrecuenciaCardiaca { get; set; }
    public double FrecuenciaRespiratoria { get; set; }
    public double TensionSistolica { get; set; }
    public double TensionDiastolica { get; set; }
}