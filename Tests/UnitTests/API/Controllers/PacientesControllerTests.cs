using API.Controllers;
using Aplicacion.Intefaces;
using Dominio.Entidades;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using AutoMapper;

namespace Tests.UnitTests.Web.Controllers;

public class PacientesControllerTests
{
    private readonly IServicioPacientes _servicioPacientes;
    private readonly PacientesController _controller;
    private readonly ILogger<PacientesController> _logger;
    private readonly IMapper _mapper;

    public PacientesControllerTests()
    {
        _servicioPacientes = Substitute.For<IServicioPacientes>();
        _logger = Substitute.For<ILogger<PacientesController>>();
        _mapper = Substitute.For<IMapper>();
        _controller = new PacientesController(_servicioPacientes, _logger, _mapper);
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

        _servicioPacientes.BuscarPacientePorCuil(Arg.Any<string>()).Returns(paciente);

        // Configure mapper to return response
        var expectedResponse = new API.DTOs.Pacientes.PacienteResponse { Cuil = cuil, Nombre = "Juan" };
        _mapper.Map<API.DTOs.Pacientes.PacienteResponse>(Arg.Any<Paciente>()).Returns(expectedResponse);

        // Act
        var resultado = _controller.BuscarPaciente(cuil);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult.Value.Should().NotBeNull();

        _servicioPacientes.Received(1).BuscarPacientePorCuil("20301234563");
    }

    [Fact]
    public void BuscarPorCuil_PacienteNoExiste_RetornaNotFound()
    {
        // Arrange
        string cuil = "20-30123456-3";
        _servicioPacientes.BuscarPacientePorCuil(Arg.Any<string>()).Returns((Paciente?)null);

        // Act
        var resultado = _controller.BuscarPaciente(cuil);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void BuscarPorCuil_ConExcepcion_LanzaExcepcion()
    {
        // Arrange
        string cuil = "20-30123456-3";

        _servicioPacientes.When(x => x.BuscarPacientePorCuil(Arg.Any<string>()))
            .Do(x => throw new Exception("Error de base de datos"));

        // Act & Assert - Exception propagates to GlobalExceptionHandler
        Action act = () => _controller.BuscarPaciente(cuil);
        act.Should().Throw<Exception>().WithMessage("*base de datos*");
    }
}