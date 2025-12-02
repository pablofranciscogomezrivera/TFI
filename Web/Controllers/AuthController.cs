using Aplicacion.Intefaces;
using Dominio.Interfaces;
using Infraestructura;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
    private readonly IConfiguration _configuration;
    private readonly IRepositorioPersonal _repoPersonal;

    public AuthController(
        IServicioAutenticacion servicioAutenticacion, 
        ILogger<AuthController> logger, 
        IConfiguration configuration, 
        IRepositorioPersonal repositorioPacientesADO)
    {
        _servicioAutenticacion = servicioAutenticacion;
        _logger = logger;
        _configuration = configuration;
        _repoPersonal = repositorioPacientesADO;
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
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            // 1. Validar credenciales
            var usuario = _servicioAutenticacion.IniciarSesion(request.Email, request.Password);

            // 2. Buscar datos del profesional (Enfermera o Médico)
            object? perfilProfesional = null;

            if (usuario.TipoAutoridad == Dominio.Enums.TipoAutoridad.Enfermera)
            {
                perfilProfesional = _repoPersonal.ObtenerEnfermeraPorUsuario(usuario.Id);
            }
            else
            {
                perfilProfesional = _repoPersonal.ObtenerDoctorPorUsuario(usuario.Id);
            }

            // 3. Generar Token JWT
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.TipoAutoridad.ToString())
            };

            //Agregar matrícula al token 
            if (perfilProfesional != null)
            {
                // Usamos reflection o dynamic porque perfilProfesional es object
                // Una forma segura es castear:
                var matricula = (perfilProfesional as dynamic).Matricula;
                claims.Add(new Claim("Matricula", matricula));
            }

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("Usuario autenticado: {Email}", usuario.Email);

            // 4. Retornar TODO al Frontend
            return Ok(new
            {
                Token = tokenString,
                Usuario = new
                {
                    usuario.Id,
                    usuario.Email,
                    usuario.TipoAutoridad
                },
                Profesional = perfilProfesional // Esto enviará Nombre, Apellido, Matricula, etc.
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Login fallido: {Message}", ex.Message);
            return Unauthorized(new ErrorResponse { Message = ex.Message, StatusCode = 401 });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login");
            return StatusCode(500, new ErrorResponse { Message = "Error interno del servidor", StatusCode = 500 });
        }
    }
}
