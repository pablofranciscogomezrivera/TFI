using Aplicacion.Interfaces;
using BCrypt.Net;

namespace Infraestructura.Servicios;

/// <summary>
/// Implementacion de IPasswordHasher para mantener la separacion de interfaces
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
