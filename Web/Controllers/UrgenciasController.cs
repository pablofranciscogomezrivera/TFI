using API.Extensions;
using API.Helpers;
using Aplicacion.Intefaces;
using Aplicacion.DTOs;
using Dominio.Entidades;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.Common;
using API.DTOs.Urgencias;
using AutoMapper;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrgenciasController : ControllerBase
{
    private readonly IServicioUrgencias _servicioUrgencias;
    private readonly ILogger<UrgenciasController> _logger;
    private readonly IMapper _mapper;

    public UrgenciasController(
        IServicioUrgencias servicioUrgencias,
        ILogger<UrgenciasController> logger,
        IMapper mapper)
    {
        _servicioUrgencias = servicioUrgencias;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Registra una nueva urgencia en el sistema
    /// </summary>
    [Authorize(Roles = "Enfermera")]
    [HttpPost]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult RegistrarUrgencia([FromBody] RegistrarUrgenciaRequest request)
    {
        var matriculaEnfermera = User.GetMatricula();

        _logger.LogInformation("=== REGISTRO DE URGENCIA ===");
        _logger.LogInformation("Matrícula Enfermera: {Matricula}, CUIL: {Cuil}", matriculaEnfermera, request.CuilPaciente);

        var cuilNormalizado = CuilHelper.Normalize(request.CuilPaciente);

        var enfermera = new Enfermera
        {
            Nombre = "Enfermera",
            Apellido = "Sistema",
            Matricula = matriculaEnfermera
        };

        // Usar AutoMapper para convertir Request -> DTO
        var dto = _mapper.Map<NuevaUrgenciaDto>(request);
        dto.CuilPaciente = cuilNormalizado; // Aplicar normalización

        _servicioUrgencias.RegistrarUrgencia(dto, enfermera);

        _logger.LogInformation("Urgencia registrada para paciente: {Cuil}", request.CuilPaciente);

        return CreatedAtAction(nameof(ObtenerListaEspera), null, new { Message = "Urgencia registrada exitosamente" });
    }

    /// <summary>
    /// Obtiene la lista de espera de pacientes pendientes
    /// </summary>
    [Authorize]
    [HttpGet("lista-espera")]
    [ProducesResponseType(typeof(List<IngresoResponse>), 200)]
    public IActionResult ObtenerListaEspera()
    {
        var ingresos = _servicioUrgencias.ObtenerIngresosPendientes();
        var response = _mapper.Map<List<IngresoResponse>>(ingresos);

        _logger.LogInformation("Lista de espera consultada. Pacientes pendientes: {Count}", response.Count);

        return Ok(response);
    }

    /// <summary>
    /// Obtiene los ingresos en proceso (reclamados por médicos pero no finalizados)
    /// </summary>
    [Authorize]
    [HttpGet("en-proceso")]
    [ProducesResponseType(typeof(List<IngresoResponse>), 200)]
    public IActionResult ObtenerIngresosEnProceso()
    {
        var todosIngresos = _servicioUrgencias.ObtenerTodosLosIngresos();
        var ingresosEnProceso = todosIngresos
            .Where(i => i.Estado == EstadoIngreso.EN_PROCESO)
            .ToList();

        var response = _mapper.Map<List<IngresoResponse>>(ingresosEnProceso);

        _logger.LogInformation("Ingresos en proceso consultados. Total: {Count}", response.Count);

        return Ok(response);
    }

    /// <summary>
    /// Reclama el siguiente paciente de la lista de espera
    /// </summary>
    [Authorize(Roles = "Medico")]
    [HttpPost("reclamar")]
    [ProducesResponseType(typeof(IngresoResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult ReclamarPaciente()
    {
        var matriculaDoctor = User.GetMatricula();

        _logger.LogInformation("Doctor reclama paciente - Matrícula: {Matricula}", matriculaDoctor);

        var doctor = new Doctor
        {
            Nombre = "Doctor",
            Apellido = "Sistema",
            Matricula = matriculaDoctor
        };

        var ingreso = _servicioUrgencias.ReclamarPaciente(doctor);
        var response = _mapper.Map<IngresoResponse>(ingreso);

        _logger.LogInformation("Paciente reclamado por doctor {Matricula}: {Cuil}", matriculaDoctor, ingreso.Paciente.CUIL);

        return Ok(response);
    }

    /// <summary>
    /// Cancela una atención médica en proceso y devuelve el paciente a la lista de espera
    /// </summary>
    [Authorize(Roles = "Medico")]
    [HttpPost("cancelar/{cuil}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult CancelarAtencion(string cuil)
    {
        var cuilNormalizado = CuilHelper.Normalize(cuil);
        _logger.LogInformation("Cancelando atención para paciente: {Cuil}", cuilNormalizado);

        _servicioUrgencias.CancelarAtencion(cuilNormalizado);

        _logger.LogInformation("Atención cancelada para paciente: {Cuil}", cuilNormalizado);

        return Ok(new { Message = "Atención cancelada exitosamente. El paciente ha vuelto a la lista de espera." });
    }
}
