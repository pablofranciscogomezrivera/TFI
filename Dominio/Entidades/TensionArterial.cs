using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class TensionArterial
{
    public Frecuencia FrecuenciaSistolica {  get; set; }
    public Frecuencia FrecuenciaDiastolica { get; set; }

    public TensionArterial(double frecuenciaSistolica, double frecuenciaDiastolica)
    {
        FrecuenciaSistolica!.Valor = frecuenciaSistolica;
        FrecuenciaDiastolica!.Valor = frecuenciaDiastolica;
    }

    public override string ToString()
    {
        var cadena = $"{FrecuenciaSistolica}/{FrecuenciaDiastolica}";
        return cadena;
    }
}

