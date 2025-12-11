namespace Aplicacion.Interfaces;

/// <summary>
/// Abstraccion para las operaciones de hashing
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string hash);
}
