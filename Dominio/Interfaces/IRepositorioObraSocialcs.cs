using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces;

public interface IRepositorioObraSocial
{
    void AgregarObraSocial(ObraSocial obraSocial);
    ObraSocial? BuscarObraSocialPorId(int id);
    bool ExisteObraSocial(int id);
    bool EstaAfiliadoAObraSocial(int obraSocialId, string numeroAfiliado);

    List<ObraSocial> ObtenerTodas();
}
