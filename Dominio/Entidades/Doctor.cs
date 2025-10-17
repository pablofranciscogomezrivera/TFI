using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class Doctor : Persona
{
    public string Matricula {  get; set; }
    public Usuario Usuario { get; set; }
}

