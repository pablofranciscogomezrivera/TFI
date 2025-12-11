using API.Controllers;
using API.DTOs.Urgencias;
using Aplicacion.Intefaces;
using Aplicacion.DTOs;
using Dominio.Entidades;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Claims;
using Xunit;
using AutoMapper;
using Dominio.Enums;

namespace Tests.UnitTests.Web.Controllers;

public class UrgenciasControllerTests
{
    private readonly IServicioUrgencias _servicioUrgencias;
    private readonly UrgenciasController _controller;
    private readonly ILogger<UrgenciasController> _logger;
    private readonly IMapper _mapper;

    public UrgenciasControllerTests()
    {
        _servicioUrgencias = Substitute.For<IServicioUrgencias>();
        _logger = Substitute.For<ILogger<UrgenciasController>>();
        _mapper = Substitute.For<IMapper>();
        _controller = new UrgenciasController(_servicioUrgencias, _logger, _mapper);

        var claims = new List<Claim>
        {
            new Claim("Matricula", "TEST_MATRICULA"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Email, "test@hospital.com"),
            new Claim(ClaimTypes.Role, "Enfermera")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public void ObtenerIngresosPendientes_ConIngresos_RetornaOk()
    {
        // Arrange
        var ingresos = new List<Ingreso>
        {
            new Ingreso(
                new Paciente { CUIL = "20-30123456-3", Nombre = "Juan", Apellido = "Pérez" },
                new Enfermera { Nombre = "María", Matricula = "ENF001" },
                "Dolor de cabeza", NivelEmergencia.URGENCIA,
                37.0, 80, 18, 120, 80
            )
        };
        _servicioUrgencias.ObtenerIngresosPendientes().Returns(ingresos);

        // Configure mapper to return response
        var expectedResponse = new List<IngresoResponse>
        {
            new IngresoResponse { CuilPaciente = "20-30123456-3" }
        };
        _mapper.Map<List<IngresoResponse>>(Arg.Any<List<Ingreso>>()).Returns(expectedResponse);

        // Act
        var resultado = _controller.ObtenerListaEspera();

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult.Value.Should().BeAssignableTo<List<IngresoResponse>>();
    }

    [Fact]
    public void RegistrarUrgencia_ConDatosValidos_RetornaOk()
    {
        // Arrange
        var request = new RegistrarUrgenciaRequest
        {
            CuilPaciente = "20-30123456-3",
            Informe = "Dolor de cabeza",
            Temperatura = 37.0,
            NivelEmergencia = NivelEmergencia.URGENCIA,
            FrecuenciaCardiaca = 80,
            FrecuenciaRespiratoria = 18,
            FrecuenciaSistolica = 120,
            FrecuenciaDiastolica = 80
        };

        // Configure mapper to return a DTO
        var expectedDto = new NuevaUrgenciaDto
        {
            CuilPaciente = request.CuilPaciente,
            Informe = request.Informe,
            Temperatura = request.Temperatura,
            NivelEmergencia = request.NivelEmergencia,
            FrecuenciaCardiaca = request.FrecuenciaCardiaca,
            FrecuenciaRespiratoria = request.FrecuenciaRespiratoria,
            FrecuenciaSistolica = request.FrecuenciaSistolica,
            FrecuenciaDiastolica = request.FrecuenciaDiastolica
        };
        _mapper.Map<NuevaUrgenciaDto>(Arg.Any<RegistrarUrgenciaRequest>()).Returns(expectedDto);

        // Act
        var resultado = _controller.RegistrarUrgencia(request);

        // Assert
        resultado.Should().BeOfType<CreatedAtActionResult>();

        _servicioUrgencias.Received(1).RegistrarUrgencia(
            Arg.Is<NuevaUrgenciaDto>(dto =>
                dto.CuilPaciente == "20301234563" && // CUIL Normalizado
                dto.Informe == request.Informe),
            Arg.Any<Enfermera>()
        );
    }

    [Fact]
    public void RegistrarUrgencia_ConDatosInvalidos_LanzaExcepcion()
    {
        // Arrange
        var request = new RegistrarUrgenciaRequest
        {
            CuilPaciente = "20-30123456-3",
            Informe = "",
            Temperatura = 37.0,
            NivelEmergencia = NivelEmergencia.URGENCIA,
            FrecuenciaCardiaca = 80,
            FrecuenciaRespiratoria = 18,
            FrecuenciaSistolica = 120,
            FrecuenciaDiastolica = 80
        };

        // Configure mapper
        var expectedDto = new NuevaUrgenciaDto { Informe = "" };
        _mapper.Map<NuevaUrgenciaDto>(Arg.Any<RegistrarUrgenciaRequest>()).Returns(expectedDto);

        _servicioUrgencias.When(x => x.RegistrarUrgencia(
            Arg.Any<NuevaUrgenciaDto>(), Arg.Any<Enfermera>()
        )).Do(x => throw new ArgumentException("El informe es mandatorio"));

        // Act & Assert - Exception propagates to GlobalExceptionHandler
        Action act = () => _controller.RegistrarUrgencia(request);
        act.Should().Throw<ArgumentException>().WithMessage("*informe*");
    }

    [Fact]
    public void ReclamarPaciente_ConPacienteDisponible_RetornaOk()
    {
        // Arrange
        var ingreso = new Ingreso(
            new Paciente { CUIL = "20-30123456-3", Nombre = "Juan" },
            new Enfermera { Nombre = "María", Matricula = "ENF001" },
            "Consulta", NivelEmergencia.URGENCIA,
            37.0, 80, 18, 120, 80
        );
        ingreso.Estado = EstadoIngreso.EN_PROCESO;

        _servicioUrgencias.ReclamarPaciente(Arg.Any<Doctor>()).Returns(ingreso);

        // Act
        var resultado = _controller.ReclamarPaciente();

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ReclamarPaciente_SinPacientesEnCola_LanzaExcepcion()
    {
        // Arrange
        _servicioUrgencias.When(x => x.ReclamarPaciente(Arg.Any<Doctor>()))
            .Do(x => throw new InvalidOperationException("No hay pacientes en la lista de espera"));

        // Act & Assert - Exception propagates to GlobalExceptionHandler
        Action act = () => _controller.ReclamarPaciente();
        act.Should().Throw<InvalidOperationException>().WithMessage("*lista de espera*");
    }

    [Fact]
    public void CancelarAtencion_ConIngresoEnProceso_RetornaOk()
    {
        // Arrange
        string cuil = "20-30123456-3";
        string cuilNormalizado = "20301234563"; // Lo que espera el servicio

        // Act
        var resultado = _controller.CancelarAtencion(cuil);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();

        _servicioUrgencias.Received(1).CancelarAtencion(cuilNormalizado);
    }

    [Fact]
    public void CancelarAtencion_SinIngresoEnProceso_LanzaExcepcion()
    {
        // Arrange
        string cuil = "20-30123456-3";
        _servicioUrgencias.When(x => x.CancelarAtencion(Arg.Any<string>()))
            .Do(x => throw new InvalidOperationException("No se encontró ingreso"));

        // Act & Assert - Exception propagates to GlobalExceptionHandler
        Action act = () => _controller.CancelarAtencion(cuil);
        act.Should().Throw<InvalidOperationException>().WithMessage("*ingreso*");
    }
}