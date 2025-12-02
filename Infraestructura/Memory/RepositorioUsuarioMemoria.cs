using Dominio.Entidades;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Memory;

public class RepositorioUsuarioMemoria : IRepositorioUsuario
{
    private readonly Dictionary<string, Usuario> _usuarios = new Dictionary<string, Usuario>(StringComparer.OrdinalIgnoreCase);
    private int _nextId = 1;

    public Usuario? BuscarPorEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        _usuarios.TryGetValue(email, out var usuario);
        return usuario;
    }

    public Usuario GuardarUsuario(Usuario usuario)
    {
        if (usuario.Id == 0)
        {
            usuario.Id = _nextId++;
        }

        _usuarios[usuario.Email] = usuario;
        return usuario;
    }

    public bool ExisteUsuarioConEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return _usuarios.ContainsKey(email);
    }
}
