using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public abstract class Persona
{
    public int DNI { get; set; }
    public int CUIL { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Email { get; set; }
    public int Telefono { get; set; }
    public string NombreCompleto { get; set; }
    public Direccion Direccion { get; set; }
    public string Localidad { get; set; }
}

