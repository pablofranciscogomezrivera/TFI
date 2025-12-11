using Aplicacion.Intefaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.DTOs.Auth;
using API.DTOs.Common;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IServicioAutenticacion _servicioAutenticacion;
    private readonly IServicioPersonal _servicioPersonal;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(
        IServicioAutenticacion servicioAutenticacion,
        IServicioPersonal servicioPersonal,
        ILogger<AuthController> logger,
        IConfiguration configuration)
    {
        _servicioAutenticacion = servicioAutenticacion;
        _servicioPersonal = servicioPersonal;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema junto con su perfil de empleado
    /// </summary>
    /// <param name="request">Datos del usuario y empleado a registrar</param>
    /// <returns>Usuario registrado</returns>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(UsuarioResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult Registrar([FromBody] RegistrarUsuarioRequest request)
    {
        var usuario = _servicioAutenticacion.RegistrarUsuarioConEmpleado(
            request.Email,
            request.Password,
            request.TipoAutoridad,
            request.Nombre,
            request.Apellido,
            request.DNI,
            request.CUIL,
            request.Matricula,
            request.FechaNacimiento,
            request.Telefono
        );

        var response = new UsuarioResponse
        {
            Id = usuario.Id,
            Email = usuario.Email,
            TipoAutoridad = usuario.TipoAutoridad
        };

        _logger.LogInformation("Usuario y empleado registrados exitosamente: {Email}", usuario.Email);

        return CreatedAtAction(nameof(Registrar), new { id = usuario.Id }, response);
    }

    /// <summary>
    /// Inicia sesión en el sistema
    /// </summary>
    /// <param name="request">Credenciales de acceso</param>
    /// <returns>Usuario autenticado</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var usuario = _servicioAutenticacion.IniciarSesion(request.Email, request.Password);

        _logger.LogInformation("Buscando profesional para usuario {UserId}, tipo: {TipoAutoridad}",
            usuario.Id, usuario.TipoAutoridad);

        var perfilProfesional = _servicioPersonal.ObtenerPerfilEmpleado(usuario.Id, usuario.TipoAutoridad);

        if (perfilProfesional == null)
        {
            throw new InvalidOperationException("Error en el perfil de usuario. Contacte al administrador.");
        }

        _logger.LogInformation("Perfil profesional encontrado para usuario {UserId}", usuario.Id);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.TipoAutoridad.ToString())
        };

        if (perfilProfesional != null)
        {
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

        return Ok(new
        {
            Token = tokenString,
            Usuario = new
            {
                usuario.Id,
                usuario.Email,
                usuario.TipoAutoridad
            },
            Profesional = perfilProfesional
        });
    }
}