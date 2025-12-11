using Dominio.Entidades.ValueObject;
using Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class Ingreso : IComparable<Ingreso>
{
    public Paciente Paciente { get; set; }
    public Enfermera Enfermera { get; set; }
    public NivelEmergencia NivelEmergencia { get; set; }
    public EstadoIngreso Estado { get; set; }
    public DateTime FechaIngreso { get; set; }

    public string InformeIngreso { get; set; }

    public Temperatura Temperatura { get; set; }
    public TensionArterial TensionArterial { get; set; }
    public FrecuenciaCardiaca FrecuenciaCardiaca { get; set; }
    public FrecuenciaRespiratoria FrecuenciaRespiratoria { get; set; }

    
    public Atencion? Atencion { get; set; }

    public Ingreso(Paciente paciente, Enfermera enfermera,
        string informeIngreso, NivelEmergencia nivelEmergencia,
        double temperatura, double frecCardiaca, double frecRespiratoria,
        double frecSistolica, double frecDiastolica)
    {
        Paciente = paciente;
        Enfermera = enfermera;
        InformeIngreso = informeIngreso;
        NivelEmergencia = nivelEmergencia;
        Temperatura = new Temperatura(temperatura);
        FrecuenciaCardiaca = new FrecuenciaCardiaca(frecCardiaca);
        FrecuenciaRespiratoria = new FrecuenciaRespiratoria(frecRespiratoria);
        TensionArterial = new TensionArterial(frecSistolica, frecDiastolica);
        FechaIngreso = DateTime.Now;
        Estado = EstadoIngreso.PENDIENTE;
        Atencion = null; 
    }

    public int CompareTo(Ingreso? ingreso)
    {
        if (ingreso == null) return 1;

        int comparacionPrioridad = NivelEmergencia.CompareTo(ingreso.NivelEmergencia);

        if (comparacionPrioridad == 0)
        {
            return FechaIngreso.CompareTo(ingreso.FechaIngreso);
        }

        return comparacionPrioridad;
    }
}
