using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Intefaces;

public interface IServicioAtencion
{
    Atencion RegistrarAtencion(Ingreso ingreso, string informeMedico, Doctor doctor);
}
