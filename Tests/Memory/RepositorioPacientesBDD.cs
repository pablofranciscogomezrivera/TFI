using Dominio.Entidades;
using Dominio.Interfaces;

namespace Tests.Memory;

/// <summary>
/// Repositorio en memoria SOLO para tests BDD.
/// Los tests unitarios usan NSubstitute.
/// </summary>
public class RepositorioPacientesBDD : IRepositorioPacientes
{
    private readonly List<Paciente> _pacientes = new();

    public Paciente? BuscarPacientePorCuil(string cuil)
    {
        return _pacientes.FirstOrDefault(p => p.CUIL == cuil);
    }

    public Paciente RegistrarPaciente(Paciente paciente)
    {
        _pacientes.Add(paciente);
        return paciente;
    }

    public void GuardarPaciente(Paciente paciente)
    {
        if (!_pacientes.Any(p => p.CUIL == paciente.CUIL))
        {
            _pacientes.Add(paciente);
        }
    }

    public List<Paciente> ObtenerTodos()
    {
        return _pacientes.ToList();
    }
}