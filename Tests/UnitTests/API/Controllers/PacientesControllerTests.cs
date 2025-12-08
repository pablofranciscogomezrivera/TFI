using API.Controllers;
using Aplicacion.Intefaces;
using Castle.Core.Logging;
using Dominio.Entidades;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Tests.UnitTests.Web.Controllers;

public class PacientesControllerTests
{
    private readonly IServicioPacientes _servicioPacientes;
    private readonly PacientesController _controller;
    private readonly ILogger<PacientesController> _logger;


    public PacientesControllerTests()
    {
        _servicioPacientes = Substitute.For<IServicioPacientes>();
        _logger = Substitute.For<ILogger<PacientesController>>();
        _controller = new PacientesController(_servicioPacientes, _logger);
    }

    [Fact]
    public void BuscarPorCuil_PacienteExiste_RetornaOk()
    {
        // Arrange
        string cuil = "20-30123456-3";
        var paciente = new Paciente
        {
            CUIL = cuil,
            DNI = 30123456,
            Nombre = "Juan",
            Apellido = "Pérez",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Domicilio = new Domicilio { Calle = "San Martín", Numero = 123, Localidad = "Tucumán" }
        };
        _servicioPacientes.BuscarPacientePorCuil(cuil).Returns(paciente);

        // Act
        var resultado = _controller.BuscarPaciente(cuil);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult.Value.Should().NotBeNull();
        _servicioPacientes.Received(1).BuscarPacientePorCuil(cuil);
    }

    [Fact]
    public void BuscarPorCuil_PacienteNoExiste_RetornaNotFound()
    {
        // Arrange
        string cuil = "20-30123456-3";
        _servicioPacientes.BuscarPacientePorCuil(cuil).Returns((Paciente?)null);

        // Act
        var resultado = _controller.BuscarPaciente(cuil);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void BuscarPorCuil_ConExcepcion_RetornaInternalServerError()
    {
        // Arrange
        string cuil = "20-30123456-3";
        _servicioPacientes.When(x => x.BuscarPacientePorCuil(cuil))
            .Do(x => throw new Exception("Error de base de datos"));

        // Act
        var resultado = _controller.BuscarPaciente(cuil);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();
        var objectResult = resultado as ObjectResult;
        objectResult.StatusCode.Should().Be(500);
    }
}