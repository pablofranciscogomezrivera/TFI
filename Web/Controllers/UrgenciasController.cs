using Aplicacion.Intefaces;
using Dominio.Entidades;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.DTOs.Common;
using Web.DTOs.Urgencias;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrgenciasController : ControllerBase
{
    private readonly IServicioUrgencias _servicioUrgencias;
    private readonly IRepositorioPacientes _repositorioPacientes;
    private readonly ILogger<UrgenciasController> _logger;

    public UrgenciasController(
        IServicioUrgencias servicioUrgencias,
        IRepositorioPacientes repositorioPacientes,
        ILogger<UrgenciasController> logger)
    {
        _servicioUrgencias = servicioUrgencias;
        _repositorioPacientes = repositorioPacientes;
        _logger = logger;
    }

    /// <summary>
    /// Registra una nueva urgencia en el sistema
    /// </summary>
    /// <param name="request">Datos de la urgencia</param>
    /// <param name="matriculaEnfermera">Matrícula de la enfermera que registra</param>
    /// <returns>Confirmación del registro</returns>
    [Authorize(Roles = "Enfermera")]
    [HttpPost]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult RegistrarUrgencia([FromBody] RegistrarUrgenciaRequest request, [FromHeader(Name = "X-Enfermera-Matricula")] string matriculaEnfermera)
    {
        try
        {
            // TODO: En una implementación real, deberías obtener la enfermera del token de autenticación
            // Por ahora creamos una enfermera mock
            var enfermera = new Enfermera
            {
                Nombre = "Enfermera",
                Apellido = "Sistema",
                Matricula = matriculaEnfermera
            };

            _servicioUrgencias.RegistrarUrgencia(
                request.CuilPaciente,
                enfermera,
                request.Informe,
                request.Temperatura,
                request.NivelEmergencia,
                request.FrecuenciaCardiaca,
                request.FrecuenciaRespiratoria,
                request.FrecuenciaSistolica,
                request.FrecuenciaDiastolica
            );

            _logger.LogInformation("Urgencia registrada para paciente: {Cuil}", request.CuilPaciente);

            return CreatedAtAction(nameof(ObtenerListaEspera), null, new { Message = "Urgencia registrada exitosamente" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Error al registrar urgencia: {Message}", ex.Message);
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al registrar urgencia");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error interno del servidor",
                StatusCode = 500
            });
        }
    }

    /// <summary>
    /// Obtiene la lista de espera de pacientes pendientes
    /// </summary>
    /// <returns>Lista de ingresos pendientes ordenados por prioridad</returns>
    [Authorize]
    [HttpGet("lista-espera")]
    [ProducesResponseType(typeof(List<IngresoResponse>), 200)]
    public IActionResult ObtenerListaEspera()
    {
        try
        {
            var ingresos = _servicioUrgencias.ObtenerIngresosPendientes();

            var response = ingresos.Select(i => new IngresoResponse
            {
                CuilPaciente = i.Paciente.CUIL,
                NombrePaciente = i.Paciente.Nombre,
                ApellidoPaciente = i.Paciente.Apellido,
                InformeInicial = i.Atencion.Informe,
                NivelEmergencia = i.NivelEmergencia,
                Estado = i.Estado,
                FechaIngreso = i.FechaIngreso,
                SignosVitales = new SignosVitalesDto
                {
                    Temperatura = i.Temperatura.Valor,
                    FrecuenciaCardiaca = i.FrecuenciaCardiaca.Valor,
                    FrecuenciaRespiratoria = i.FrecuenciaRespiratoria.Valor,
                    TensionSistolica = i.TensionArterial.FrecuenciaSistolica.Valor,
                    TensionDiastolica = i.TensionArterial.FrecuenciaDiastolica.Valor
                }
            }).ToList();

            _logger.LogInformation("Lista de espera consultada. Pacientes pendientes: {Count}", response.Count());

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener lista de espera");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error interno del servidor",
                StatusCode = 500
            });
        }
    }

    /// <summary>
    /// Reclama el siguiente paciente de la lista de espera
    /// </summary>
    /// <param name="matriculaDoctor">Matrícula del doctor que reclama</param>
    /// <returns>Datos del ingreso reclamado</returns>
    [Authorize(Roles = "Medico")]
    [HttpPost("reclamar")]
    [ProducesResponseType(typeof(IngresoResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult ReclamarPaciente([FromHeader(Name = "X-Doctor-Matricula")] string matriculaDoctor)
    {
        try
        {
            // TODO: En una implementación real, deberías obtener el doctor del token de autenticación
            // Por ahora creamos un doctor mock
            var doctor = new Doctor
            {
                Nombre = "Doctor",
                Apellido = "Sistema",
                Matricula = matriculaDoctor
            };

            var ingreso = _servicioUrgencias.ReclamarPaciente(doctor);

            var response = new IngresoResponse
            {
                CuilPaciente = ingreso.Paciente.CUIL,
                NombrePaciente = ingreso.Paciente.Nombre,
                ApellidoPaciente = ingreso.Paciente.Apellido,
                InformeInicial = ingreso.Atencion.Informe,
                NivelEmergencia = ingreso.NivelEmergencia,
                Estado = ingreso.Estado,
                FechaIngreso = ingreso.FechaIngreso,
                SignosVitales = new SignosVitalesDto
                {
                    Temperatura = ingreso.Temperatura.Valor,
                    FrecuenciaCardiaca = ingreso.FrecuenciaCardiaca.Valor,
                    FrecuenciaRespiratoria = ingreso.FrecuenciaRespiratoria.Valor,
                    TensionSistolica = ingreso.TensionArterial.FrecuenciaSistolica.Valor,
                    TensionDiastolica = ingreso.TensionArterial.FrecuenciaDiastolica.Valor
                }
            };

            _logger.LogInformation("Paciente reclamado por doctor {Matricula}: {Cuil}", matriculaDoctor, ingreso.Paciente.CUIL);

            return Ok(response);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogWarning("Error al reclamar paciente: {Message}", ex.Message);
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("No hay pacientes en lista de espera");
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al reclamar paciente");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error interno del servidor",
                StatusCode = 500
            });
        }
    }
}
