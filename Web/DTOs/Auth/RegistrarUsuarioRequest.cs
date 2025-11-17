using Dominio.Enums;

namespace Webb.DTOs.Auth;

public class RegistrarUsuarioRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public TipoAutoridad TipoAutoridad { get; set; }
}
