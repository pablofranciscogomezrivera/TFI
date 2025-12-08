using API.Controllers;
using API.DTOs.Urgencias;
using Aplicacion.Intefaces;
using Dominio.Entidades;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Claims;
using Xunit;

namespace Tests.UnitTests.API.Controllers;

public class UrgenciasControllerTests
{
    private readonly IServicioUrgencias _servicioUrgencias;
    private readonly UrgenciasController _controller;
    private readonly ILogger<UrgenciasController> _logger;

    public UrgenciasControllerTests()
    {
        _servicioUrgencias = Substitute.For<IServicioUrgencias>();
        _logger = Substitute.For<ILogger<UrgenciasController>>();
        _controller = new UrgenciasController(_servicioUrgencias, _logger);

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

        // Act
        var resultado = _controller.RegistrarUrgencia(request);

        // Assert
        resultado.Should().BeOfType<CreatedAtActionResult>(); 

        _servicioUrgencias.Received(1).RegistrarUrgencia(
            "20301234563", 
            Arg.Any<Enfermera>(),
            request.Informe,
            request.Temperatura,
            Arg.Any<NivelEmergencia>(),
            request.FrecuenciaCardiaca,
            request.FrecuenciaRespiratoria,
            request.FrecuenciaSistolica,
            request.FrecuenciaDiastolica
        );
    }

    [Fact]
    public void RegistrarUrgencia_ConDatosInvalidos_RetornaBadRequest()
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

        _servicioUrgencias.When(x => x.RegistrarUrgencia(
            Arg.Any<string>(), Arg.Any<Enfermera>(), Arg.Any<string>(),
            Arg.Any<double>(), Arg.Any<NivelEmergencia>(),
            Arg.Any<double>(), Arg.Any<double>(), Arg.Any<double>(), Arg.Any<double>()
        )).Do(x => throw new ArgumentException("El informe es mandatorio"));

        // Act
        var resultado = _controller.RegistrarUrgencia(request);

        // Assert
        resultado.Should().BeOfType<BadRequestObjectResult>();
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
    public void ReclamarPaciente_SinPacientesEnCola_RetornaBadRequest()
    {
        // Arrange
        _servicioUrgencias.When(x => x.ReclamarPaciente(Arg.Any<Doctor>()))
            .Do(x => throw new InvalidOperationException("No hay pacientes en la lista de espera"));

        // Act
        var resultado = _controller.ReclamarPaciente();

        // Assert
        resultado.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void CancelarAtencion_ConIngresoEnProceso_RetornaOk()
    {
        // Arrange
        string cuil = "20-30123456-3";
        string cuilNormalizado = "20301234563"; 

        // Act
        var resultado = _controller.CancelarAtencion(cuil);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();

        _servicioUrgencias.Received(1).CancelarAtencion(cuilNormalizado);
    }

    [Fact]
    public void CancelarAtencion_SinIngresoEnProceso_RetornaBadRequest()
    {
        // Arrange
        string cuil = "20-30123456-3";
        _servicioUrgencias.When(x => x.CancelarAtencion(Arg.Any<string>()))
            .Do(x => throw new InvalidOperationException("No se encontró ingreso"));

        // Act
        var resultado = _controller.CancelarAtencion(cuil);

        // Assert
        resultado.Should().BeOfType<BadRequestObjectResult>();
    }
}