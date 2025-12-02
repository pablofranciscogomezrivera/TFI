using Microsoft.AspNetCore.Mvc;
using Aplicacion.Intefaces;
using Dominio.Entidades;
using Dominio.Interfaces;
using Web.DTOs.Atenciones;
using Web.DTOs.Common;
using Infraestructura.Memory;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AtencionesController : ControllerBase
{
    private readonly IServicioAtencion _servicioAtencion;
    private readonly IRepositorioUrgencias _repositorioUrgencias;
    private readonly ILogger<AtencionesController> _logger;

    public AtencionesController(
        IServicioAtencion servicioAtencion,
        IRepositorioUrgencias repositorioUrgencias,
        ILogger<AtencionesController> logger)
    {
        _servicioAtencion = servicioAtencion;
        _repositorioUrgencias = repositorioUrgencias;
        _logger = logger;
    }

    /// <summary>
    /// Registra la atención médica de un paciente
    /// </summary>
    /// <param name="cuilPaciente">CUIL del paciente</param>
    /// <param name="request">Informe médico</param>
    /// <param name="matriculaDoctor">Matrícula del doctor</param>
    /// <returns>Datos de la atención registrada</returns>
    [HttpPost("{cuilPaciente}")]
    [ProducesResponseType(typeof(AtencionResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public IActionResult RegistrarAtencion(
        string cuilPaciente,
        [FromBody] RegistrarAtencionRequest request,
        [FromHeader(Name = "X-Doctor-Matricula")] string matriculaDoctor)
    {
        try
        {
            // Buscar el ingreso EN_PROCESO del paciente
            // Nota: En una implementación real, deberías tener una forma mejor de trackear
            // qué ingreso está atendiendo cada doctor. Aquí simplificamos buscando todos
            // los ingresos y filtrando.
            var todosIngresos = ObtenerTodosLosIngresos();
            var ingreso = todosIngresos
                .FirstOrDefault(i => i.Paciente.CUIL == cuilPaciente && i.Estado == EstadoIngreso.EN_PROCESO);

            if (ingreso == null)
            {
                _logger.LogWarning("No se encontró ingreso en proceso para paciente: {Cuil}", cuilPaciente);
                return NotFound(new ErrorResponse
                {
                    Message = $"No se encontró un ingreso en proceso para el paciente con CUIL {cuilPaciente}",
                    StatusCode = 404
                });
            }

            // TODO: En una implementación real, deberías obtener el doctor del token de autenticación
            var doctor = new Doctor
            {
                Nombre = "Doctor",
                Apellido = "Sistema",
                Matricula = matriculaDoctor
            };

            var atencion = _servicioAtencion.RegistrarAtencion(
                ingreso,
                request.InformeMedico,
                doctor
            );

            var response = new AtencionResponse
            {
                CuilPaciente = ingreso.Paciente.CUIL,
                NombrePaciente = ingreso.Paciente.Nombre,
                ApellidoPaciente = ingreso.Paciente.Apellido,
                Doctor = $"{atencion.Doctor.Nombre} {atencion.Doctor.Apellido}",
                MatriculaDoctor = atencion.Doctor.Matricula,
                InformeCompleto = atencion.Informe
            };

            _logger.LogInformation(
                "Atención registrada para paciente {Cuil} por doctor {Matricula}",
                cuilPaciente,
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

    /// <summary>
    /// Obtiene todos los ingresos del repositorio (helper method)
    /// En una implementación real, esto debería estar en el repositorio
    /// </summary>
    private List<Ingreso> ObtenerTodosLosIngresos()
    {
        // Usamos reflexión para acceder a la lista privada del repositorio
        // Esto es un workaround temporal. En producción deberías agregar un método
        // en IRepositorioUrgencias para obtener ingresos por estado o por paciente
        var repositorio = _repositorioUrgencias as RepositorioUrgenciasMemoria;
        if (repositorio != null)
        {
            var field = repositorio.GetType().GetField("_ingresos",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var ingresos = field?.GetValue(repositorio) as List<Ingreso>;
            return ingresos ?? new List<Ingreso>();
        }
        return new List<Ingreso>();
    }
}
