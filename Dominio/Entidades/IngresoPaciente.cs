using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class IngresoPaciente
{
    public DateTime FechaIngreso { get; set; }
    public string Informe { get; set; }
    public NivelEmergencia NivelEmergencia { get; set; }
    public Estado Estado { get; set; }
    public double Temperatura { get; set; }
    public double FrecuenciaCardiaca { get; set; }
    public double FrecuenciaRespiratorioa { get; set; }
    public TensionArterial TensionArterial { get; set; }  

    public Enfermera Enfermera { get; set; }
}
