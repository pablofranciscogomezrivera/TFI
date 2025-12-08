using Dominio.Entidades;

namespace Dominio.Interfaces;

public interface IRepositorioPersonal
{
    Enfermera? ObtenerEnfermeraPorUsuario(int idUsuario);
    Doctor? ObtenerDoctorPorUsuario(int idUsuario);
    void GuardarEnfermera(Enfermera enfermera, int idUsuario);
    void GuardarDoctor(Doctor doctor, int idUsuario);
}