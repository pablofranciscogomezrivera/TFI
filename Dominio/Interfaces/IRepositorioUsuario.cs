using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces;

public interface IRepositorioUsuario
{
    Usuario? BuscarPorEmail(string email);
    Usuario GuardarUsuario(Usuario usuario);
    bool ExisteUsuarioConEmail(string email);
}
