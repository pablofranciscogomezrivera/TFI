using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class FrecuenciaRespiratoria : Frecuencia
{
    public FrecuenciaRespiratoria(double valor) : base(valor)
    {
    }

    protected override Exception NotificarError()
    {
        return new ArgumentException("La frecuencia respiratoria no puede ser negativa");
    }
}

