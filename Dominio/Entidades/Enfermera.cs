namespace Dominio.Entidades;

public class Enfermera : Persona
{
    public required string Matricula {  get; set; }
    public required Usuario Usuario { get; set; }
}
