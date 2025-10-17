using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public abstract class Persona
{
    public required int DNI { get; set; }
    public required string CUIL { get; set; }
    public required DateTime FechaNacimiento { get; set; }
    public required string Email { get; set; }
    public required int Telefono { get; set; }
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
}   

