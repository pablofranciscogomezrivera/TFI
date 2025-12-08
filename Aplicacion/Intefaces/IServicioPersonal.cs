using Dominio.Entidades;

namespace Aplicacion.Intefaces;

public interface IServicioPersonal
{
    /// <summary>
    /// Obtiene el perfil de empleado (Doctor o Enfermera) asociado a un usuario
    /// </summary>
    /// <param name="idUsuario">ID del usuario</param>
    /// <param name="tipoAutoridad">Tipo de autoridad (Medico o Enfermera)</param>
    /// <returns>Doctor o Enfermera según corresponda, null si no existe</returns>
    object? ObtenerPerfilEmpleado(int idUsuario, Dominio.Enums.TipoAutoridad tipoAutoridad);

    /// <summary>
    /// Obtiene una enfermera por su ID de usuario
    /// </summary>
    Enfermera? ObtenerEnfermeraPorUsuario(int idUsuario);

    /// <summary>
    /// Obtiene un doctor por su ID de usuario
    /// </summary>
    Doctor? ObtenerDoctorPorUsuario(int idUsuario);
}