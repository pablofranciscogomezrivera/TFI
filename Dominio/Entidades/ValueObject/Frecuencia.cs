﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.ValueObject;

public abstract class Frecuencia
{
    public  double Valor {  get; set; }

    protected Frecuencia(double valor)
    {
        if (valor < 0)
        {
            throw NotificarError();
        }
        Valor = valor;
    }

    protected abstract Exception NotificarError();
}

