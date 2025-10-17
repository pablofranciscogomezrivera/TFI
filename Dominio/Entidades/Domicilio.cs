using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades;

public class Domicilio
{
    public required string Calle {  get; set; }
    public required int Numero { get; set; }
    public required string Ciudad { get; set; }
    public required string Provincia { get; set; }
    public required string Localidad { get; set; }
}

