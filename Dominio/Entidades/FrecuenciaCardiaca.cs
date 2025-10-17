using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class FrecuenciaCardiaca : Frecuencia
{
    public FrecuenciaCardiaca(double valor) : base(valor)
    {
    }

    protected override Exception NotificarError()
    {
        return new ArgumentException("La frecuencia cardiaca no puede ser negativa");
    }
}

