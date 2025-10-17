using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class Ingreso
{
    public Atencion Atencion { get; set; }
    public Paciente Paciente { get; set; }
    public Enfermera Enfermera { get; set; }
    public NivelEmergencia NivelEmergencia { get; set; }
    public EstadoIngreso Estado { get; set; }
    public DateTime FechaIngreso { get; set; }
    public Temperatura Temperatura { get; set; }
    public TensionArterial TensionArterial { get; set; }
    public FrecuenciaCardiaca FrecuenciaCardiaca { get; set; }
    public FrecuenciaRespiratoria FrecuenciaRespiratoria { get; set; }

    public Ingreso(Paciente paciente, Enfermera enfermera,
        string informe, NivelEmergencia nivelEmergencia,
        double temperatura, double frecCardiaca, double frecRespiratoria,
        double frecSistolica, double frecDiastolica)
    {
        Paciente = paciente;
        Enfermera = enfermera;
        Atencion = new Atencion();
        Atencion.Informe = informe;
        NivelEmergencia = nivelEmergencia;
        Temperatura = new Temperatura();
        Temperatura.Valor = temperatura;
        FrecuenciaCardiaca = new FrecuenciaCardiaca(frecCardiaca);
        FrecuenciaRespiratoria = new FrecuenciaRespiratoria(frecRespiratoria);
        TensionArterial = new TensionArterial(frecSistolica, frecDiastolica);
        FechaIngreso = DateTime.Now;
        Estado = EstadoIngreso.PENDIENTE;
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
