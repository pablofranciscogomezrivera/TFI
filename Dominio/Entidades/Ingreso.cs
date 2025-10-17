using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class Ingreso
{
    public required Atencion Atencion { get; set; }
    public required Paciente Paciente { get; set; }
    public required Enfermera Enfermera { get; set; }
    public required NivelEmergencia NivelEmergencia { get; set; }
    public required EstadoIngreso Estado { get; set; }
    public required string Descripcion { get; set; }
    public DateTime FechaIngreso { get; set; }
    public required Temperatura Temperatura { get; set; }
    public required TensionArterial TensionArterial { get; set; }
    public required FrecuenciaCardiaca FrecuenciaCardiaca { get; set; }
    public required FrecuenciaRespiratoria FrecuenciaRespiratoria {  get; set; }
    
}
