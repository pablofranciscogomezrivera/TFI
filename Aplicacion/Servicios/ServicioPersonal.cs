using Aplicacion.Intefaces;
using Dominio.Entidades;
using Dominio.Enums;
using Dominio.Interfaces;

namespace Aplicacion.Servicios;

public class ServicioPersonal : IServicioPersonal
{
    private readonly IRepositorioPersonal _repositorioPersonal;

    public ServicioPersonal(IRepositorioPersonal repositorioPersonal)
    {
        _repositorioPersonal = repositorioPersonal ?? throw new ArgumentNullException(nameof(repositorioPersonal));
    }

    public object? ObtenerPerfilEmpleado(int idUsuario, TipoAutoridad tipoAutoridad)
    {
        if (idUsuario <= 0)
        {
            throw new ArgumentException("El ID de usuario debe ser mayor a 0", nameof(idUsuario));
        }

        return tipoAutoridad switch
        {
            TipoAutoridad.Enfermera => ObtenerEnfermeraPorUsuario(idUsuario),
            TipoAutoridad.Medico => ObtenerDoctorPorUsuario(idUsuario),
            _ => throw new ArgumentException($"Tipo de autoridad no reconocido: {tipoAutoridad}", nameof(tipoAutoridad))
        };
    }

    public Enfermera? ObtenerEnfermeraPorUsuario(int idUsuario)
    {
        if (idUsuario <= 0)
        {
            throw new ArgumentException("El ID de usuario debe ser mayor a 0", nameof(idUsuario));
        }

        return _repositorioPersonal.ObtenerEnfermeraPorUsuario(idUsuario);
    }

    public Doctor? ObtenerDoctorPorUsuario(int idUsuario)
    {
        if (idUsuario <= 0)
        {
            throw new ArgumentException("El ID de usuario debe ser mayor a 0", nameof(idUsuario));
        }

        return _repositorioPersonal.ObtenerDoctorPorUsuario(idUsuario);
    }
}