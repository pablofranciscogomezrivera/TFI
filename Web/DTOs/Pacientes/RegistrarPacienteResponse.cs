namespace Webb.DTOs.Pacientes;

public class RegistrarPacienteRequest
{
    public string Cuil { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Calle { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string Localidad { get; set; } = string.Empty;
    public int? ObraSocialId { get; set; }
    public string? NumeroAfiliado { get; set; }
    public DateTime FechaNacimiento { get; set; }
}