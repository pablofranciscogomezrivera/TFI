using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class Doctor : Persona
{
    public required string Matricula {  get; set; }
    public required Usuario Usuario { get; set; }
}

