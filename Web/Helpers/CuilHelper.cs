namespace API.Helpers;

/// <summary>
/// Helpers para trabajar con CUILs
/// </summary>
public static class CuilHelper
{
    /// <summary>
    /// Normaliza un CUIL removiendo guiones y espacios
    /// </summary>
    /// <param name="cuil">CUIL con o sin guiones</param>
    /// <returns>CUIL solo con dígitos</returns>
    public static string Normalize(string cuil)
    {
        if (string.IsNullOrWhiteSpace(cuil))
            return cuil;

        return cuil.Replace("-", "").Replace(" ", "").Trim();
    }

    /// <summary>
    /// Formatea un CUIL agregando guiones
    /// </summary>
    /// <param name="cuil">CUIL sin guiones</param>
    /// <returns>CUIL formateado XX-XXXXXXXX-X</returns>
    public static string Format(string cuil)
    {
        if (string.IsNullOrWhiteSpace(cuil))
            return cuil;

        var normalized = Normalize(cuil);

        if (normalized.Length != 11)
            return cuil; // Retornar original si no tiene 11 dígitos

        return $"{normalized.Substring(0, 2)}-{normalized.Substring(2, 8)}-{normalized.Substring(10, 1)}";
    }
}
