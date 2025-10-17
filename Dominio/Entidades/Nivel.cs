using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class Nivel
{
    public int Nivell { get; set; }
    public TimeOnly DuracionMaxEspera { get; set; }
    public NivelEmergencia NivelEmergencia { get; set; }
}

