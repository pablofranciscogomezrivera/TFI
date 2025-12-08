using Dominio.Entidades;
using Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Intefaces;

public interface IServicioAutenticacion
{
    Usuario RegistrarUsuario(string email, string password, TipoAutoridad tipoAutoridad);
    Usuario RegistrarUsuarioConEmpleado(
        string email,
        string password,
        TipoAutoridad tipoAutoridad,
        string nombre,
        string apellido,
        int dni,
        string cuil,
        string matricula,
        DateTime? fechaNacimiento = null,
        long? telefono = null);
    Usuario IniciarSesion(string email, string password);
}