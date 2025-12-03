using System.Security.Claims;

namespace Web.Extensions;

/// <summary>
/// Extensiones para extraer información de forma segura desde los Claims del usuario autenticado
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Obtiene la matrícula del profesional desde los claims del JWT
    /// </summary>
    /// <param name="user">ClaimsPrincipal del usuario autenticado</param>
    /// <returns>Matrícula del profesional</returns>
    /// <exception cref="UnauthorizedAccessException">Si no se encuentra la matrícula en los claims</exception>
    public static string GetMatricula(this ClaimsPrincipal user)
    {
        var matriculaClaim = user.FindFirst("Matricula");

        if (matriculaClaim == null || string.IsNullOrWhiteSpace(matriculaClaim.Value))
        {
            throw new UnauthorizedAccessException("No se encontró la matrícula en el token de autenticación. Por favor, inicie sesión nuevamente.");
        }

        return matriculaClaim.Value;
    }

    /// <summary>
    /// Obtiene el ID del usuario desde los claims del JWT
    /// </summary>
    /// <param name="user">ClaimsPrincipal del usuario autenticado</param>
    /// <returns>ID del usuario</returns>
    /// <exception cref="UnauthorizedAccessException">Si no se encuentra el ID en los claims</exception>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            throw new UnauthorizedAccessException("No se encontró el ID de usuario en el token de autenticación.");
        }

        return userId;
    }

    /// <summary>
    /// Obtiene el email del usuario desde los claims del JWT
    /// </summary>
    /// <param name="user">ClaimsPrincipal del usuario autenticado</param>
    /// <returns>Email del usuario</returns>
    /// <exception cref="UnauthorizedAccessException">Si no se encuentra el email en los claims</exception>
    public static string GetEmail(this ClaimsPrincipal user)
    {
        var emailClaim = user.FindFirst(ClaimTypes.Email);

        if (emailClaim == null || string.IsNullOrWhiteSpace(emailClaim.Value))
        {
            throw new UnauthorizedAccessException("No se encontró el email en el token de autenticación.");
        }

        return emailClaim.Value;
    }

    /// <summary>
    /// Obtiene el rol del usuario desde los claims del JWT
    /// </summary>
    /// <param name="user">ClaimsPrincipal del usuario autenticado</param>
    /// <returns>Rol del usuario</returns>
    /// <exception cref="UnauthorizedAccessException">Si no se encuentra el rol en los claims</exception>
    public static string GetRole(this ClaimsPrincipal user)
    {
        var roleClaim = user.FindFirst(ClaimTypes.Role);

        if (roleClaim == null || string.IsNullOrWhiteSpace(roleClaim.Value))
        {
            throw new UnauthorizedAccessException("No se encontró el rol en el token de autenticación.");
        }

        return roleClaim.Value;
    }

    /// <summary>
    /// Intenta obtener la matrícula del profesional, retornando null si no existe
    /// </summary>
    /// <param name="user">ClaimsPrincipal del usuario autenticado</param>
    /// <returns>Matrícula del profesional o null si no existe</returns>
    public static string? TryGetMatricula(this ClaimsPrincipal user)
    {
        var matriculaClaim = user.FindFirst("Matricula");
        return matriculaClaim?.Value;
    }

    /// <summary>
    /// Verifica si el usuario tiene un claim específico
    /// </summary>
    /// <param name="user">ClaimsPrincipal del usuario autenticado</param>
    /// <param name="claimType">Tipo de claim a verificar</param>
    /// <returns>True si el claim existe, false en caso contrario</returns>
    public static bool HasClaim(this ClaimsPrincipal user, string claimType)
    {
        return user.FindFirst(claimType) != null;
    }
}
