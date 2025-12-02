using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

/// <summary>
/// Value Object para representar temperatura corporal en grados Celsius
/// </summary>
public class Temperatura
{
    public double Valor { get; private set; }

    public Temperatura()
    {
        Valor = 0;
    }

    public Temperatura(double valor)
    {
        Valor = valor;
    }
}

