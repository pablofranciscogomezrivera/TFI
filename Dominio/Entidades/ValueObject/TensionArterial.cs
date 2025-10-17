using Dominio.Entidades.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.ValueObject;

public record TensionArterial
{
    public FrecuenciaSistolica FrecuenciaSistolica { get; }
    public FrecuenciaDiastolica FrecuenciaDiastolica { get; }

    public TensionArterial(double valorSistolica, double valorDiastolica)
    {
        FrecuenciaSistolica = new FrecuenciaSistolica(valorSistolica);
        FrecuenciaDiastolica = new FrecuenciaDiastolica(valorDiastolica);
    }

    public override string ToString()
    {
        return $"{FrecuenciaSistolica.Valor}/{FrecuenciaDiastolica.Valor}";
    }
}

