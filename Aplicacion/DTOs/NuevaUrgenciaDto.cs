using Dominio.Entidades;

namespace Aplicacion.DTOs;

public class NuevaUrgenciaDto
{
    public string CuilPaciente { get; set; } = string.Empty;
    public string Informe { get; set; } = string.Empty;
    public NivelEmergencia NivelEmergencia { get; set; }

    // Signos Vitales
    public double Temperatura { get; set; }
    public double FrecuenciaCardiaca { get; set; }
    public double FrecuenciaRespiratoria { get; set; }
    public double FrecuenciaSistolica { get; set; }
    public double FrecuenciaDiastolica { get; set; }

    // Datos del Paciente (Opcional, agrupado)
    public DatosNuevoPacienteDto? DatosPaciente { get; set; }
}