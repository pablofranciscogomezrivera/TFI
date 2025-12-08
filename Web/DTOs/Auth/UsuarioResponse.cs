using Dominio.Enums;

namespace API.DTOs.Auth;

public class UsuarioResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public TipoAutoridad TipoAutoridad { get; set; }
}