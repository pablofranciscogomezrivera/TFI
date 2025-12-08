namespace API.DTOs.Atenciones;

public class AtencionResponse
{
    public string CuilPaciente { get; set; } = string.Empty;
    public string NombrePaciente { get; set; } = string.Empty;
    public string ApellidoPaciente { get; set; } = string.Empty;
    public string Doctor { get; set; } = string.Empty;
    public string MatriculaDoctor { get; set; } = string.Empty;
    public string InformeCompleto { get; set; } = string.Empty;
}