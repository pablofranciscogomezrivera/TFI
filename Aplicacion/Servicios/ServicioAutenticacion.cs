using Aplicacion.Intefaces;
using Dominio.Entidades;
using Dominio.Enums;
using Dominio.Interfaces;
using Dominio.Validadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Aplicacion;

public class ServicioAutenticacion : IServicioAutenticacion
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IRepositorioPersonal _repositorioPersonal;

    public ServicioAutenticacion(IRepositorioUsuario repositorioUsuario, IRepositorioPersonal repositorioPersonal)
    {
        _repositorioUsuario = repositorioUsuario;
        _repositorioPersonal = repositorioPersonal;
    }

    public Usuario RegistrarUsuario(string email, string password, TipoAutoridad tipoAutoridad)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email es un campo mandatorio");
        }

        if (!ValidadorEmail.EsValido(email))
        {
            throw new ArgumentException("El email no tiene un formato válido");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("La contraseña es un campo mandatorio");
        }

        if (password.Length < 8)
        {
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres");
        }

        if (_repositorioUsuario.ExisteUsuarioConEmail(email))
        {
            throw new ArgumentException("Ya existe un usuario registrado con ese email");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var usuario = new Usuario
        {
            Email = email,
            PasswordHash = passwordHash,
            TipoAutoridad = tipoAutoridad
        };

        return _repositorioUsuario.GuardarUsuario(usuario);
    }

    public Usuario RegistrarUsuarioConEmpleado(
        string email,
        string password,
        TipoAutoridad tipoAutoridad,
        string nombre,
        string apellido,
        int dni,
        string cuil,
        string matricula,
        DateTime? fechaNacimiento = null,
        long? telefono = null)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email es un campo mandatorio");
        }

        if (!ValidadorEmail.EsValido(email))
        {
            throw new ArgumentException("El email no tiene un formato válido");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("La contraseña es un campo mandatorio");
        }

        if (password.Length < 8)
        {
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres");
        }

        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre es un campo mandatorio");
        }

        if (string.IsNullOrWhiteSpace(apellido))
        {
            throw new ArgumentException("El apellido es un campo mandatorio");
        }

        if (dni <= 0)
        {
            throw new ArgumentException("El DNI debe ser un número válido");
        }

        if (string.IsNullOrWhiteSpace(cuil))
        {
            throw new ArgumentException("El CUIL es un campo mandatorio");
        }

        if (!ValidadorCUIL.EsValido(cuil))
        {
            throw new ArgumentException("El CUIL no tiene un formato válido");
        }

        if (string.IsNullOrWhiteSpace(matricula))
        {
            throw new ArgumentException("La matrícula es un campo mandatorio");
        }

        if (_repositorioUsuario.ExisteUsuarioConEmail(email))
        {
            throw new ArgumentException("Ya existe un usuario registrado con ese email");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var usuario = new Usuario
        {
            Email = email,
            PasswordHash = passwordHash,
            TipoAutoridad = tipoAutoridad
        };

        var usuarioGuardado = _repositorioUsuario.GuardarUsuario(usuario);

        if (tipoAutoridad == TipoAutoridad.Enfermera)
        {
            var enfermera = new Enfermera
            {
                Nombre = nombre,
                Apellido = apellido,
                DNI = dni,
                CUIL = cuil,
                Matricula = matricula,
                Email = email,
                Telefono = telefono ?? 0,
                FechaNacimiento = fechaNacimiento ?? DateTime.MinValue
            };

            _repositorioPersonal.GuardarEnfermera(enfermera, usuarioGuardado.Id);
        }
        else 
        {
            var doctor = new Doctor
            {
                Nombre = nombre,
                Apellido = apellido,
                DNI = dni,
                CUIL = cuil,
                Matricula = matricula,
                Email = email,
                Telefono = telefono ?? 0,
                FechaNacimiento = fechaNacimiento ?? DateTime.MinValue
            };

            _repositorioPersonal.GuardarDoctor(doctor, usuarioGuardado.Id);
        }

        return usuarioGuardado;
    }

    public Usuario IniciarSesion(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Usuario o contraseña inválidos");
        }

        var usuario = _repositorioUsuario.BuscarPorEmail(email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
        {
            throw new ArgumentException("Usuario o contraseña inválidos");
        }

        return usuario;
    }
}