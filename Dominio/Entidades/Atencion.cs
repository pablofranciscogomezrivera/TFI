using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class Atencion
{
    public required Doctor Doctor { get; set; }
    public required string Informe { get; set; }
}

