namespace Web.DTOs.Atenciones;

public class RegistrarAtencionRequest
{
    public string CuilPaciente { get; set; } = string.Empty;
    public string InformeMedico { get; set; } = string.Empty;
}