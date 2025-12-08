using Dominio.Enums;

namespace API.DTOs.Auth;

public class RegistrarUsuarioRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public TipoAutoridad TipoAutoridad { get; set; }

    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public int DNI { get; set; }
    public string CUIL { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public long? Telefono { get; set; }
}
