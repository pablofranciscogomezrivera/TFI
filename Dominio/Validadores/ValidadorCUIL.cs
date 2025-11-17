using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dominio.Validadores;

public static class ValidadorCUIL
{
    public static bool EsValido(string cuil)
    {
        if (string.IsNullOrWhiteSpace(cuil))
            return false;

        // Eliminar guiones y espacios
        cuil = cuil.Replace("-", "").Replace(" ", "");

        // Verificar que tenga 11 dígitos
        if (cuil.Length != 11 || !cuil.All(char.IsDigit))
            return false;

        // Validar el dígito verificador
        int[] multiplicadores = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
        int suma = 0;

        for (int i = 0; i < 10; i++)
        {
            suma += int.Parse(cuil[i].ToString()) * multiplicadores[i];
        }

        int resto = suma % 11;
        int digitoVerificador = resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;

        return digitoVerificador == int.Parse(cuil[10].ToString());
    }
}
