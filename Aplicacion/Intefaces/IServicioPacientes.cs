using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Intefaces;

public interface IServicioPacientes
{
    Paciente RegistrarPaciente(
        string cuil,
        string nombre,
        string apellido,
        string calle,
        int numero,
        string localidad,
        int? obraSocialId = null,
        string? numeroAfiliado = null);
}
