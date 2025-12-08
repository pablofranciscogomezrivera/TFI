using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Validadores;

public static class ValidadorCUIL
{
    public static bool EsValido(string cuil)
    {
        if (string.IsNullOrWhiteSpace(cuil)) return false;

        cuil = cuil.Replace("-", "").Replace(" ", "").Trim();

        if (cuil.Length != 11 || !long.TryParse(cuil, out _)) return false;

        int[] multiplicadores = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
        int suma = 0;

        for (int i = 0; i < 10; i++)
        {
            suma += (cuil[i] - '0') * multiplicadores[i];
        }

        int resto = suma % 11;
        int digitoCalculado = 11 - resto;

        if (digitoCalculado == 11) digitoCalculado = 0;

        if (digitoCalculado == 10) return false;

        int digitoVerificadorReal = cuil[10] - '0';

        return digitoCalculado == digitoVerificadorReal;
    }
}