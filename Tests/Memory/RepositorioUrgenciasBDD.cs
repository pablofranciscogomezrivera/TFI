using Dominio.Entidades;
using Dominio.Interfaces;

namespace Tests.Memory;

/// <summary>
/// Repositorio en memoria SOLO para tests BDD.
/// Los tests unitarios usan NSubstitute.
/// </summary>
public class RepositorioUrgenciasBDD : IRepositorioUrgencias
{
    private readonly List<Ingreso> _ingresos = new();

    public void AgregarIngreso(Ingreso ingreso)
    {
        _ingresos.Add(ingreso);
    }

    public List<Ingreso> ObtenerIngresosPendientes()
    {
        return _ingresos
            .Where(i => i.Estado == EstadoIngreso.PENDIENTE)
            .OrderBy(i => i)
            .ToList();
    }

    public Ingreso? ObtenerIngresoPorCuilYEstado(string cuil, EstadoIngreso estado)
    {
        return _ingresos.FirstOrDefault(i => i.Paciente.CUIL == cuil && i.Estado == estado);
    }

    public void ActualizarIngreso(Ingreso ingreso)
    {
        return;
    }

    public List<Ingreso> ObtenerTodos()
    {
        return _ingresos.ToList();
    }

    public void RemoverIngreso(Ingreso ingreso)
    {
        throw new NotImplementedException();
    }

    public Ingreso? BuscarIngresoPorCuilYEstado(string cuil, EstadoIngreso estado)
    {
        return _ingresos.FirstOrDefault(i => i.Paciente.CUIL == cuil && i.Estado == estado);
    }

    public List<Ingreso> ObtenerTodosLosIngresos()
    {
        throw new NotImplementedException();
    }

    public Ingreso? ObtenerSiguienteIngresoPendiente()
    {
        return _ingresos
            .Where(i => i.Estado == EstadoIngreso.PENDIENTE)
            .OrderBy(i => i)
            .FirstOrDefault();
    }

    public bool IntentarAsignarMedico(Ingreso ingreso, Doctor doctor)
    {
        var candidato = _ingresos.FirstOrDefault(i =>
            i.Paciente.CUIL == ingreso.Paciente.CUIL &&
            i.FechaIngreso == ingreso.FechaIngreso);

        if (candidato != null && candidato.Estado == EstadoIngreso.PENDIENTE)
        {
            candidato.Estado = EstadoIngreso.EN_PROCESO;
            if (candidato.Atencion == null)
            {
                candidato.Atencion = new Atencion();
            }
            candidato.Atencion.Doctor = doctor;
            return true;
        }
        return false;
    }
}