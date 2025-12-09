using API.Extensions;
using API.Helpers;
using Aplicacion.Intefaces;
using Dominio.Entidades;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.Common;
using API.DTOs.Urgencias;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrgenciasController : ControllerBase
{
    private readonly IServicioUrgencias _servicioUrgencias;
    private readonly ILogger<UrgenciasController> _logger;

    public UrgenciasController(
        IServicioUrgencias servicioUrgencias,
        ILogger<UrgenciasController> logger)
    {
        _servicioUrgencias = servicioUrgencias;
        _logger = logger;
    }

    /// <summary>
    /// Registra una nueva urgencia en el sistema
    /// </summary>
    /// <param name="request">Datos de la urgencia</param>
    /// <returns>Confirmación del registro</returns>
    [Authorize(Roles = "Enfermera")]
    [HttpPost]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult RegistrarUrgencia([FromBody] RegistrarUrgenciaRequest request)
    {
        try
        {

            var matriculaEnfermera = User.GetMatricula();

            _logger.LogInformation("=== REGISTRO DE URGENCIA ===");
            _logger.LogInformation("Matrícula Enfermera (desde JWT): {Matricula}", matriculaEnfermera);
            _logger.LogInformation("CUIL: {Cuil}", request.CuilPaciente);
            _logger.LogInformation("Informe: {Informe}", request.Informe);
            _logger.LogInformation("Nivel Emergencia: {Nivel}", request.NivelEmergencia);
            _logger.LogInformation("Temperatura: {Temp}", request.Temperatura);
            _logger.LogInformation("FC: {FC}, FR: {FR}", request.FrecuenciaCardiaca, request.FrecuenciaRespiratoria);
            _logger.LogInformation("TA: {Sist}/{Diast}", request.FrecuenciaSistolica, request.FrecuenciaDiastolica);
            _logger.LogInformation("Paciente Nuevo - Nombre: {Nombre}, Apellido: {Apellido}",
                request.NombrePaciente ?? "null", request.ApellidoPaciente ?? "null");


            var cuilNormalizado = CuilHelper.Normalize(request.CuilPaciente);
            _logger.LogInformation("CUIL normalizado: {CuilOriginal} -> {CuilNormalizado}",
                request.CuilPaciente, cuilNormalizado);


            var emailEnfermera = User.GetEmail();


            var enfermera = new Enfermera
            {
                Nombre = "Enfermera",
                Apellido = "Sistema",
                Matricula = matriculaEnfermera
            };

            _servicioUrgencias.RegistrarUrgencia(
                cuilNormalizado,
                enfermera,
                request.Informe,
                request.Temperatura,
                request.NivelEmergencia,
                request.FrecuenciaCardiaca,
                request.FrecuenciaRespiratoria,
                request.FrecuenciaSistolica,
                request.FrecuenciaDiastolica,
                // Datos opcionales del paciente
                request.NombrePaciente,
                request.ApellidoPaciente,
                request.CallePaciente,
                request.NumeroPaciente,
                request.LocalidadPaciente,
                request.EmailPaciente,
                request.TelefonoPaciente,
                request.ObraSocialIdPaciente,
                request.NumeroAfiliadoPaciente,
                request.FechaNacimientoPaciente
            );

            _logger.LogInformation("Urgencia registrada para paciente: {Cuil}", request.CuilPaciente);

            return CreatedAtAction(nameof(ObtenerListaEspera), null, new { Message = "Urgencia registrada exitosamente" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Error de validación al registrar urgencia: {Message}", ex.Message);
            _logger.LogWarning("StackTrace: {StackTrace}", ex.StackTrace);
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al registrar urgencia: {Message}", ex.Message);
            _logger.LogError("StackTrace: {StackTrace}", ex.StackTrace);
            return StatusCode(500, new ErrorResponse
            {
                Message = $"Error interno: {ex.Message}",
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
                InformeInicial = i.InformeIngreso,
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
    /// Obtiene los ingresos en proceso (reclamados por médicos pero no finalizados)
    /// </summary>
    /// <returns>Lista de ingresos en proceso</returns>
    [Authorize]
    [HttpGet("en-proceso")]
    [ProducesResponseType(typeof(List<IngresoResponse>), 200)]
    public IActionResult ObtenerIngresosEnProceso()
    {
        try
        {
            var todosIngresos = _servicioUrgencias.ObtenerTodosLosIngresos();
            var ingresosEnProceso = todosIngresos
                .Where(i => i.Estado == EstadoIngreso.EN_PROCESO)
                .ToList();

            var response = ingresosEnProceso.Select(i => new IngresoResponse
            {
                CuilPaciente = i.Paciente.CUIL,
                NombrePaciente = i.Paciente.Nombre,
                ApellidoPaciente = i.Paciente.Apellido,
                InformeInicial = i.InformeIngreso,
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

            _logger.LogInformation("Ingresos en proceso consultados. Total: {Count}", response.Count);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ingresos en proceso");
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
    /// <returns>Datos del ingreso reclamado</returns>
    [Authorize(Roles = "Medico")]
    [HttpPost("reclamar")]
    [ProducesResponseType(typeof(IngresoResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult ReclamarPaciente()
    {
        try
        {
            var matriculaDoctor = User.GetMatricula();

            _logger.LogInformation("Doctor reclama paciente - Matrícula (desde JWT): {Matricula}", matriculaDoctor);

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
                InformeInicial = ingreso.InformeIngreso,
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

    /// <summary>
    /// Cancela una atención médica en proceso y devuelve el paciente a la lista de espera
    /// </summary>
    /// <param name="cuil">CUIL del paciente</param>
    /// <returns>Confirmación de cancelación</returns>
    [Authorize(Roles = "Medico")]
    [HttpPost("cancelar/{cuil}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult CancelarAtencion(string cuil)
    {
        try
        {
            var cuilNormalizado = CuilHelper.Normalize(cuil);
            _logger.LogInformation("Cancelando atención para paciente: {Cuil}", cuilNormalizado);

            _servicioUrgencias.CancelarAtencion(cuilNormalizado);

            _logger.LogInformation("Atención cancelada para paciente: {Cuil}", cuilNormalizado);

            return Ok(new { Message = "Atención cancelada exitosamente. El paciente ha vuelto a la lista de espera." });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Error al cancelar atención: {Message}", ex.Message);
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Error al cancelar atención: {Message}", ex.Message);
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al cancelar atención");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error interno del servidor",
                StatusCode = 500
            });
        }
    }
}
