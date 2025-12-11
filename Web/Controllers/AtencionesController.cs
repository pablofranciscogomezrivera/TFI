using API.Helpers;
using Aplicacion.Intefaces;
using Dominio.Entidades;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.Atenciones;
using API.DTOs.Common;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace API.Controllers;

[Authorize(Roles = "Medico")]
[ApiController]
[Route("api/[controller]")]
public class AtencionesController : ControllerBase
{
    private readonly IServicioAtencion _servicioAtencion;
    private readonly ILogger<AtencionesController> _logger;
    private readonly IMapper _mapper;

    public AtencionesController(
        IServicioAtencion servicioAtencion,
        ILogger<AtencionesController> logger,
        IMapper mapper)
    {
        _servicioAtencion = servicioAtencion;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Registra la atención médica de un paciente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AtencionResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public IActionResult RegistrarAtencion([FromBody] RegistrarAtencionRequest request)
    {
        var matriculaDoctor = User.GetMatricula();

        _logger.LogInformation("Registrando atención - Matrícula Doctor: {Matricula}", matriculaDoctor);

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
}
