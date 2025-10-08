using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class TensionArterial
{
    public double FrecuenciaSistolica {  get; set; }
    public double FrecuenciaDiastolica { get; set; }

    public override string ToString()
    {
        var cadena = $"{FrecuenciaSistolica}/{FrecuenciaDiastolica}";
        return cadena;
    }
}

