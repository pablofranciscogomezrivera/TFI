using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class Afiliado
{
    public required ObraSocial ObraSocial { get; set; }
    public required string NumeroAfiliado { get; set; }
}

