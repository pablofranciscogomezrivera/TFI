using Dominio.Entidades;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Memory;

public class RepositorioObraSocialMemoria : IRepositorioObraSocial
{
    private readonly Dictionary<int, ObraSocial> _obrasSociales = new Dictionary<int, ObraSocial>();
    private readonly Dictionary<int, HashSet<string>> _afiliados = new Dictionary<int, HashSet<string>>();

    public void AgregarObraSocial(ObraSocial obraSocial)
    {
        _obrasSociales[obraSocial.Id] = obraSocial;
        if (!_afiliados.ContainsKey(obraSocial.Id))
        {
            _afiliados[obraSocial.Id] = new HashSet<string>();
        }
    }

    public void AgregarAfiliado(int obraSocialId, string numeroAfiliado)
    {
        if (!_afiliados.ContainsKey(obraSocialId))
        {
            _afiliados[obraSocialId] = new HashSet<string>();
        }
        _afiliados[obraSocialId].Add(numeroAfiliado);
    }

    public ObraSocial? BuscarObraSocialPorId(int id)
    {
        _obrasSociales.TryGetValue(id, out var obraSocial);
        return obraSocial;
    }

    public bool ExisteObraSocial(int id)
    {
        return _obrasSociales.ContainsKey(id);
    }

    public bool EstaAfiliadoAObraSocial(int obraSocialId, string numeroAfiliado)
    {
        if (!_afiliados.ContainsKey(obraSocialId))
            return false;

        return _afiliados[obraSocialId].Contains(numeroAfiliado);
    }

    public List<ObraSocial> ObtenerTodas()
    {
        return _obrasSociales.Values.ToList();
    }
}
