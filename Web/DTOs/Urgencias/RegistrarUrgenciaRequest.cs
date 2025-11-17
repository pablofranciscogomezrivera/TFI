using Dominio.Entidades;

namespace Web.DTOs.Urgencias;

public class RegistrarUrgenciaRequest
{
    public string CuilPaciente { get; set; } = string.Empty;
    public string Informe { get; set; } = string.Empty;
    public double Temperatura { get; set; }
    public NivelEmergencia NivelEmergencia { get; set; }
    public double FrecuenciaCardiaca { get; set; }
    public double FrecuenciaRespiratoria { get; set; }
    public double FrecuenciaSistolica { get; set; }
    public double FrecuenciaDiastolica { get; set; }
}