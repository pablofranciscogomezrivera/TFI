using Dominio.Entidades;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Memory;

public class RepositorioPacientesMemoria : IRepositorioPacientes
{
    private readonly Dictionary<string, Paciente> _pacientes = new Dictionary<string, Paciente>();
    public void GuardarPaciente(Paciente Paciente)
    {
        _pacientes[Paciente.CUIL] = Paciente;
    }

    public Paciente? BuscarPacientePorCuil(string cuil)
    {
        _pacientes.TryGetValue(cuil, out var paciente);
        return paciente;
    }

    public Paciente RegistrarPaciente(Paciente paciente)
    {
        GuardarPaciente(paciente);
        return paciente;
    }
}

