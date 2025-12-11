using Dominio.Entidades;
using Aplicacion.DTOs;

namespace Aplicacion.Intefaces;

public interface IServicioUrgencias
{
    void RegistrarUrgencia(NuevaUrgenciaDto datos, Enfermera enfermera);

    List<Ingreso> ObtenerIngresosPendientes();
    List<Ingreso> ObtenerTodosLosIngresos();
    Ingreso ReclamarPaciente(Doctor doctor);
    void CancelarAtencion(string cuilPaciente);
}
