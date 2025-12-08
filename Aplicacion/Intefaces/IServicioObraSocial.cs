using Dominio.Entidades;

namespace Aplicacion.Intefaces;

public interface IServicioObraSocial
{
    /// <summary>
    /// Obtiene todas las obras sociales disponibles
    /// </summary>
    /// <returns>Lista de obras sociales</returns>
    List<ObraSocial> ObtenerTodasLasObrasSociales();

    /// <summary>
    /// Busca una obra social por su ID
    /// </summary>
    /// <param name="id">ID de la obra social</param>
    /// <returns>Obra social encontrada o null</returns>
    ObraSocial? BuscarObraSocialPorId(int id);

    /// <summary>
    /// Verifica si existe una obra social con el ID especificado
    /// </summary>
    /// <param name="id">ID de la obra social</param>
    /// <returns>True si existe, false en caso contrario</returns>
    bool ExisteObraSocial(int id);

    /// <summary>
    /// Verifica si un paciente está afiliado a una obra social
    /// </summary>
    /// <param name="obraSocialId">ID de la obra social</param>
    /// <param name="numeroAfiliado">Número de afiliado</param>
    /// <returns>True si está afiliado, false en caso contrario</returns>
    bool EstaAfiliadoAObraSocial(int obraSocialId, string numeroAfiliado);
}