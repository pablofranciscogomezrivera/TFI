namespace API.DTOs.Pacientes;

public class PacienteResponse
{
    public string Cuil { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public DomicilioDto Domicilio { get; set; } = new();
    public AfiliadoDto? Afiliado { get; set; }
}

public class DomicilioDto
{
    public string Calle { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string Localidad { get; set; } = string.Empty;
}

public class AfiliadoDto
{
    public string NumeroAfiliado { get; set; } = string.Empty;
    public string ObraSocial { get; set; } = string.Empty;
}