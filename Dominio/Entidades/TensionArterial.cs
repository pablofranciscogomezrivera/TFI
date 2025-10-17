using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class TensionArterial
{
    public required Frecuencia FrecuenciaSistolica {  get; set; }
    public required Frecuencia FrecuenciaDiastolica { get; set; }

    public override string ToString()
    {
        var cadena = $"{FrecuenciaSistolica}/{FrecuenciaDiastolica}";
        return cadena;
    }
}

