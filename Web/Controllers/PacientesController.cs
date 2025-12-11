using Aplicacion.Intefaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.Common;
using API.DTOs.Pacientes;
using API.Helpers;
using AutoMapper;

namespace API.Controllers;

[Authorize(Roles = "Enfermera")]
[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IServicioPacientes _servicioPacientes;
    private readonly ILogger<PacientesController> _logger;
    private readonly IMapper _mapper;

    public PacientesController(
        IServicioPacientes servicioPacientes,
        ILogger<PacientesController> logger,
        IMapper mapper)
    {
        _servicioPacientes = servicioPacientes;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Registra un nuevo paciente en el sistema
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PacienteResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult RegistrarPaciente([FromBody] RegistrarPacienteRequest request)
    {
        var cuilNormalizado = CuilHelper.Normalize(request.Cuil);
        var paciente = _servicioPacientes.RegistrarPaciente(
            cuilNormalizado,
            request.Nombre,
            request.Apellido,
            request.Calle,
            request.Numero,
            request.Localidad,
            request.FechaNacimiento,
            request.ObraSocialId,
            request.NumeroAfiliado
        );

        var response = _mapper.Map<PacienteResponse>(paciente);

        _logger.LogInformation("Paciente registrado exitosamente: {Cuil}", paciente.CUIL);

        return CreatedAtAction(nameof(RegistrarPaciente), new { cuil = paciente.CUIL }, response);
    }

    /// <summary>
    /// Busca un paciente por su CUIL
    /// </summary>
    [HttpGet("{cuil}")]
    [ProducesResponseType(typeof(PacienteResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult BuscarPaciente(string cuil)
    {
        var cuilNormalizado = CuilHelper.Normalize(cuil);
        var paciente = _servicioPacientes.BuscarPacientePorCuil(cuilNormalizado);

        if (paciente == null)
        {
            return NotFound(new ErrorResponse
            {
                Message = $"No se encontró un paciente con CUIL {cuil}",
                StatusCode = 404
            });
        }

        var response = _mapper.Map<PacienteResponse>(paciente);

        _logger.LogInformation("Paciente encontrado: {Cuil}", cuil);
        return Ok(response);
    }
}
