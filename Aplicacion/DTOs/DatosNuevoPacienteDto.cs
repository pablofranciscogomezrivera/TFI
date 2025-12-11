using System;

namespace Aplicacion.DTOs;

public class DatosNuevoPacienteDto
{
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? Email { get; set; }
    public long? Telefono { get; set; }
    
    // Domicilio
    public string? Calle { get; set; }
    public int? Numero { get; set; }
    public string? Localidad { get; set; }

    // Obra Social
    public int? ObraSocialId { get; set; }
    public string? NumeroAfiliado { get; set; }
}