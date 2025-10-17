using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.ValueObject;

public class FrecuenciaSistolica : Frecuencia
{
    public FrecuenciaSistolica(double valor) : base(valor) { }

    protected override Exception NotificarError()
    {
        return new ArgumentException("La frecuencia sistolica no puede ser negativa");
    }
}