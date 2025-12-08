using Aplicacion.Intefaces;
using Dominio.Entidades;
using Dominio.Interfaces;

namespace Aplicacion.Servicios;

public class ServicioObraSocial : IServicioObraSocial
{
    private readonly IRepositorioObraSocial _repositorioObraSocial;

    public ServicioObraSocial(IRepositorioObraSocial repositorioObraSocial)
    {
        _repositorioObraSocial = repositorioObraSocial ?? throw new ArgumentNullException(nameof(repositorioObraSocial));
    }

    public List<ObraSocial> ObtenerTodasLasObrasSociales()
    {
        return _repositorioObraSocial.ObtenerTodas();
    }

    public ObraSocial? BuscarObraSocialPorId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("El ID de la obra social debe ser mayor a 0", nameof(id));
        }

        return _repositorioObraSocial.BuscarObraSocialPorId(id);
    }

    public bool ExisteObraSocial(int id)
    {
        if (id <= 0)
        {
            return false;
        }

        return _repositorioObraSocial.ExisteObraSocial(id);
    }

    public bool EstaAfiliadoAObraSocial(int obraSocialId, string numeroAfiliado)
    {
        if (obraSocialId <= 0)
        {
            throw new ArgumentException("El ID de la obra social debe ser mayor a 0", nameof(obraSocialId));
        }

        if (string.IsNullOrWhiteSpace(numeroAfiliado))
        {
            throw new ArgumentException("El número de afiliado es mandatorio", nameof(numeroAfiliado));
        }

        return _repositorioObraSocial.EstaAfiliadoAObraSocial(obraSocialId, numeroAfiliado);
    }
}