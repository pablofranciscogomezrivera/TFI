
using API.Helpers;
using Aplicacion.Intefaces;
using Dominio.Entidades;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.Atenciones;
using API.DTOs.Common;
using API.Extensions;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AtencionesController : ControllerBase
{
    private readonly IServicioAtencion _servicioAtencion;
    private readonly ILogger<AtencionesController> _logger;

    public AtencionesController(
        IServicioAtencion servicioAtencion,
        ILogger<AtencionesController> logger)
    {
        _servicioAtencion = servicioAtencion;
        _logger = logger;
    }

    /// <summary>
    /// Registra la atención médica de un paciente
    /// </summary>
    /// <param name="request">Datos de la atención (CUIL del paciente e informe médico)</param>
    /// <returns>Datos de la atención registrada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AtencionResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public IActionResult RegistrarAtencion([FromBody] RegistrarAtencionRequest request)
    {
        try
        {
            
            var matriculaDoctor = User.GetMatricula();

            _logger.LogInformation("Registrando atención - Matrícula Doctor (desde JWT): {Matricula}", matriculaDoctor);

            var cuilNormalizado = CuilHelper.Normalize(request.CuilPaciente);
            _logger.LogInformation("CUIL normalizado: {CuilOriginal} -> {CuilNormalizado}",
                request.CuilPaciente, cuilNormalizado);

            var doctor = new Doctor
            {
                Nombre = "Doctor",
                Apellido = "Sistema",
                Matricula = matriculaDoctor
            };

            var atencion = _servicioAtencion.RegistrarAtencionPorCuil(
                cuilNormalizado,
                request.InformeMedico,
                doctor
            );

            var response = new AtencionResponse
            {
                CuilPaciente = cuilNormalizado,
                NombrePaciente = atencion.Doctor.Nombre,
                ApellidoPaciente = atencion.Doctor.Apellido,
                Doctor = $"{atencion.Doctor.Nombre} {atencion.Doctor.Apellido}",
                MatriculaDoctor = atencion.Doctor.Matricula,
                InformeCompleto = atencion.Informe
            };

            _logger.LogInformation(
                "Atención registrada para paciente {Cuil} por doctor {Matricula}",
                cuilNormalizado,
                matriculaDoctor);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Error al registrar atención: {Message}", ex.Message);
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Error de operación al registrar atención: {Message}", ex.Message);
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 400
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al registrar atención");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error interno del servidor",
                StatusCode = 500
            });
        }
    }

}
