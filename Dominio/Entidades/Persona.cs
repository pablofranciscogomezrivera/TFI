using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public abstract class Persona
{
    public int DNI { get; set; }
    public string CUIL { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Email { get; set; }
    public long Telefono { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
}   

//prueba para el workflow