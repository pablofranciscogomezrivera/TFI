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

    public ServicioAutenticacion(IRepositorioUsuario repositorioUsuario)
    {
        _repositorioUsuario = repositorioUsuario;
    }

    public Usuario RegistrarUsuario(string email, string password, TipoAutoridad tipoAutoridad)
    {
        // Validar email mandatorio
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email es un campo mandatorio");
        }

        // Validar formato de email
        if (!ValidadorEmail.EsValido(email))
        {
            throw new ArgumentException("El email no tiene un formato válido");
        }

        // Validar contraseña mandatoria
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("La contraseña es un campo mandatorio");
        }

        // Validar longitud mínima de contraseña
        if (password.Length < 8)
        {
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres");
        }

        // Validar que el email no esté registrado
        if (_repositorioUsuario.ExisteUsuarioConEmail(email))
        {
            throw new ArgumentException("Ya existe un usuario registrado con ese email");
        }

        // Hashear la contraseña usando BCrypt
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        // Crear el usuario
        var usuario = new Usuario
        {
            Email = email,
            PasswordHash = passwordHash,
            TipoAutoridad = tipoAutoridad
        };

        // Guardar en el repositorio
        return _repositorioUsuario.GuardarUsuario(usuario);
    }

    public Usuario IniciarSesion(string email, string password)
    {
        // Validar que se proporcionen las credenciales
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Usuario o contraseña inválidos");
        }

        // Buscar el usuario por email
        var usuario = _repositorioUsuario.BuscarPorEmail(email);

        // Si no existe el usuario o la contraseña no coincide, lanzar el mismo error genérico
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
        {
            throw new ArgumentException("Usuario o contraseña inválidos");
        }

        return usuario;
    }
}