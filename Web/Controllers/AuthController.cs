using Aplicacion.Intefaces;
using Microsoft.AspNetCore.Mvc;
using Web.DTOs.Auth;
using Web.DTOs.Common;
using Webb.DTOs.Auth;
namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IServicioAutenticacion _servicioAutenticacion;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IServicioAutenticacion servicioAutenticacion, ILogger<AuthController> logger)
    {
        _servicioAutenticacion = servicioAutenticacion;
        _logger = logger;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema
    /// </summary>
    /// <param name="request">Datos del usuario a registrar</param>
    /// <returns>Usuario registrado</returns>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(UsuarioResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult Registrar([FromBody] RegistrarUsuarioRequest request)
    {
        try
        {
            var usuario = _servicioAutenticacion.RegistrarUsuario(
                request.Email,
                request.Password,
                request.TipoAutoridad
            );

            var response = new UsuarioResponse
            {
                Id = usuario.Id,
                Email = usuario.Email,
                TipoAutoridad = usuario.TipoAutoridad
            };

            _logger.LogInformation("Usuario registrado exitosamente: {Email}", usuario.Email);

            return CreatedAtAction(nameof(Registrar), new { id = usuario.Id }, response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Error al registrar usuario: {Message}", ex.Message);
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al registrar usuario");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error interno del servidor",
                StatusCode = 500
            });
        }
    }

    /// <summary>
    /// Inicia sesión en el sistema
    /// </summary>
    /// <param name="request">Credenciales de acceso</param>
    /// <returns>Usuario autenticado</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UsuarioResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            var usuario = _servicioAutenticacion.IniciarSesion(
                request.Email,
                request.Password
            );

            var response = new UsuarioResponse
            {
                Id = usuario.Id,
                Email = usuario.Email,
                TipoAutoridad = usuario.TipoAutoridad
            };

            _logger.LogInformation("Usuario autenticado exitosamente: {Email}", usuario.Email);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Intento de login fallido: {Message}", ex.Message);
            return Unauthorized(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 401
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al iniciar sesión");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error interno del servidor",
                StatusCode = 500
            });
        }
    }
}
