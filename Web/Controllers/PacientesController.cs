using Aplicacion.Intefaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.DTOs.Common;
using Web.DTOs.Pacientes;
using Webb.DTOs.Pacientes;

namespace Webb.Controllers;

[Authorize(Roles = "Enfermera")]
[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IServicioPacientes _servicioPacientes;
    private readonly ILogger<PacientesController> _logger;

    public PacientesController(IServicioPacientes servicioPacientes, ILogger<PacientesController> logger)
    {
        _servicioPacientes = servicioPacientes;
        _logger = logger;
    }

    /// <summary>
    /// Registra un nuevo paciente en el sistema
    /// </summary>
    /// <param name="request">Datos del paciente a registrar</param>
    /// <returns>Paciente registrado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PacienteResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult RegistrarPaciente([FromBody] RegistrarPacienteRequest request)
    {
        try
        {
            var paciente = _servicioPacientes.RegistrarPaciente(
                request.Cuil,
                request.Nombre,
                request.Apellido,
                request.Calle,
                request.Numero,
                request.Localidad,
                request.FechaNacimiento,
                request.ObraSocialId,
                request.NumeroAfiliado
            );

            var response = new PacienteResponse
            {
                Cuil = paciente.CUIL,
                Nombre = paciente.Nombre,
                Apellido = paciente.Apellido,
                Domicilio = new DomicilioDto
                {
                    Calle = paciente.Domicilio.Calle,
                    Numero = paciente.Domicilio.Numero,
                    Localidad = paciente.Domicilio.Localidad
                },
                Afiliado = paciente.Afiliado != null ? new AfiliadoDto
                {
                    NumeroAfiliado = paciente.Afiliado.NumeroAfiliado,
                    ObraSocial = paciente.Afiliado.ObraSocial.Nombre
                } : null
            };

            _logger.LogInformation("Paciente registrado exitosamente: {Cuil}", paciente.CUIL);

            return CreatedAtAction(nameof(RegistrarPaciente), new { cuil = paciente.CUIL }, response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Error al registrar paciente: {Message}", ex.Message);
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al registrar paciente");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error interno del servidor",
                StatusCode = 500
            });
        }
    }

    /// <summary>
    /// Busca un paciente por su CUIL
    /// </summary>
    /// <param name="cuil">CUIL del paciente</param>
    /// <returns>Datos del paciente si existe</returns>
    [HttpGet("{cuil}")]
    [ProducesResponseType(typeof(PacienteResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult BuscarPaciente(string cuil)
    {
        try
        {
            var paciente = _servicioPacientes.BuscarPacientePorCuil(cuil);

            if (paciente == null)
            {
                // Este 404 es el que espera tu frontend para abrir el formulario de registro
                return NotFound(new ErrorResponse
                {
                    Message = $"No se encontró un paciente con CUIL {cuil}",
                    StatusCode = 404
                });
            }

            // Mapeamos la entidad a la respuesta DTO
            var response = new PacienteResponse
            {
                Cuil = paciente.CUIL,
                Nombre = paciente.Nombre,
                Apellido = paciente.Apellido,
                Domicilio = new DomicilioDto
                {
                    Calle = paciente.Domicilio.Calle,
                    Numero = paciente.Domicilio.Numero,
                    Localidad = paciente.Domicilio.Localidad
                },
                Afiliado = paciente.Afiliado != null ? new AfiliadoDto
                {
                    NumeroAfiliado = paciente.Afiliado.NumeroAfiliado,
                    ObraSocial = paciente.Afiliado.ObraSocial.Nombre
                } : null
            };

            _logger.LogInformation("Paciente encontrado: {Cuil}", cuil);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse { Message = ex.Message, StatusCode = 400 });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar paciente");
            return StatusCode(500, new ErrorResponse { Message = "Error interno del servidor", StatusCode = 500 });
        }
    }
}
