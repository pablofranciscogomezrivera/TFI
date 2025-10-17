using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class Nivel
{
    public required int Nivell { get; set; }
    public required TimeOnly DuracionMaxEspera { get; set; }
    public required NivelEmergencia NivelEmergencia { get; set; }
}

