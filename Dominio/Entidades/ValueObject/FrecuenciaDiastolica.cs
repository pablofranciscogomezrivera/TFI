using Dominio.Entidades.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class FrecuenciaDiastolica : Frecuencia
{
    public FrecuenciaDiastolica(double valor) : base(valor) { }

    protected override Exception NotificarError()
    {
        return new ArgumentException("La frecuencia diastolica no puede ser negativa");
    }
}