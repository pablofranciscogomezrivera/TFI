using Dominio.Entidades;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura;

public class DBPruebaMemoria : IRepositorioPacientes
{
    public Dictionary<string, Paciente> Pacientes = new Dictionary<string, Paciente>();
    public void GuardarPaciente(Paciente Paciente)
    {
        Pacientes.Add(Paciente.CUIL, Paciente);
    }
}

