using Aplicacion;
using Aplicacion.Servicios;
using Dominio.Entidades;
using Dominio.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Tests.UnitTests.Aplicacion.Servicios;

public class ServicioPacientesTests
{
    private readonly IRepositorioPacientes _repositorioPacientes;
    private readonly IRepositorioObraSocial _repositorioObraSocial;
    private readonly ServicioPacientes _servicioPacientes;

    public ServicioPacientesTests()
    {
        _repositorioPacientes = Substitute.For<IRepositorioPacientes>();
        _repositorioObraSocial = Substitute.For<IRepositorioObraSocial>();
        _servicioPacientes = new ServicioPacientes(_repositorioPacientes, _repositorioObraSocial);
    }

    [Fact]
    public void RegistrarPaciente_ConDatosValidosSinObraSocial_CreaPaciente()
    {
        // Arrange
        _repositorioPacientes.RegistrarPaciente(Arg.Any<Paciente>())
            .Returns(callInfo => callInfo.Arg<Paciente>());

        // Act
        var resultado = _servicioPacientes.RegistrarPaciente(
            "20-30123456-3", "Juan", "Pérez", "San Martín", 123,
            "Tucumán", new DateTime(1990, 1, 1)
        );

        // Assert
        resultado.Should().NotBeNull();
        resultado.CUIL.Should().Be("20-30123456-3");
        resultado.Nombre.Should().Be("Juan");
        resultado.Afiliado.Should().BeNull();
        _repositorioPacientes.Received(1).RegistrarPaciente(Arg.Any<Paciente>());
    }

    [Fact]
    public void RegistrarPaciente_ConObraSocialYAfiliadoValido_CreaPacienteConAfiliado()
    {
        // Arrange
        int obraSocialId = 1;
        string numeroAfiliado = "12345";

        var obraSocial = new ObraSocial { Id = obraSocialId, Nombre = "OSDE" };

        _repositorioObraSocial.ExisteObraSocial(obraSocialId).Returns(true);
        _repositorioObraSocial.BuscarObraSocialPorId(obraSocialId).Returns(obraSocial);
        _repositorioObraSocial.EstaAfiliadoAObraSocial(obraSocialId, numeroAfiliado).Returns(true);

        _repositorioPacientes.RegistrarPaciente(Arg.Any<Paciente>())
            .Returns(callInfo => callInfo.Arg<Paciente>());

        // Act
        var resultado = _servicioPacientes.RegistrarPaciente(
            "20-30123456-3", "Juan", "Pérez", "San Martín", 123,
            "Tucumán", new DateTime(1990, 1, 1), obraSocialId, numeroAfiliado
        );

        // Assert
        resultado.Should().NotBeNull();
        resultado.Afiliado.Should().NotBeNull();
        resultado.Afiliado.NumeroAfiliado.Should().Be(numeroAfiliado);
        resultado.Afiliado.ObraSocial.Should().Be(obraSocial);

        _repositorioObraSocial.Received(1).ExisteObraSocial(obraSocialId);
        _repositorioObraSocial.Received(1).BuscarObraSocialPorId(obraSocialId);
        _repositorioObraSocial.Received(1).EstaAfiliadoAObraSocial(obraSocialId, numeroAfiliado);
    }

    [Fact]
    public void RegistrarPaciente_ConObraSocialInexistente_LanzaExcepcion()
    {
        // Arrange
        int obraSocialId = 999;

        _repositorioObraSocial.ExisteObraSocial(obraSocialId).Returns(false);
        _repositorioObraSocial.BuscarObraSocialPorId(obraSocialId).Returns((ObraSocial?)null);

        // Act
        Action act = () => _servicioPacientes.RegistrarPaciente(
            "20-30123456-3", "Juan", "Pérez", "San Martín", 123,
            "Tucumán", new DateTime(1990, 1, 1), obraSocialId, "12345"
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*obra social inexistente*");
        _repositorioPacientes.DidNotReceive().RegistrarPaciente(Arg.Any<Paciente>());
    }

    [Fact]
    public void RegistrarPaciente_NoAfiliadoAObraSocial_LanzaExcepcion()
    {
        // Arrange
        int obraSocialId = 1;
        string numeroAfiliado = "99999";

        var obraSocial = new ObraSocial { Id = obraSocialId, Nombre = "OSDE" };

        _repositorioObraSocial.ExisteObraSocial(obraSocialId).Returns(true);
        _repositorioObraSocial.BuscarObraSocialPorId(obraSocialId).Returns(obraSocial);
        _repositorioObraSocial.EstaAfiliadoAObraSocial(obraSocialId, numeroAfiliado).Returns(false);

        // Act
        Action act = () => _servicioPacientes.RegistrarPaciente(
            "20-30123456-3", "Juan", "Pérez", "San Martín", 123,
            "Tucumán", new DateTime(1990, 1, 1), obraSocialId, numeroAfiliado
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*no está afiliado*");
    }

    [Theory]
    [InlineData("", "Juan", "Pérez", "El CUIL es un campo mandatorio")]
    [InlineData("20-30123456-3", "", "Pérez", "El Nombre es un campo mandatorio")]
    [InlineData("20-30123456-3", "Juan", "", "El Apellido es un campo mandatorio")]
    public void RegistrarPaciente_ConDatosMandatoriosVacios_LanzaExcepcion(
        string cuil, string nombre, string apellido, string mensajeEsperado)
    {
        // Act
        Action act = () => _servicioPacientes.RegistrarPaciente(
            cuil, nombre, apellido, "Calle", 123, "Localidad", new DateTime(1990, 1, 1)
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage(mensajeEsperado);
    }

    [Fact]
    public void BuscarPacientePorCuil_PacienteExiste_RetornaPaciente()
    {
        // Arrange
        string cuil = "20-30123456-3";
        var paciente = new Paciente { CUIL = cuil, Nombre = "Juan" };
        _repositorioPacientes.BuscarPacientePorCuil(cuil).Returns(paciente);

        // Act
        var resultado = _servicioPacientes.BuscarPacientePorCuil(cuil);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(paciente);
        _repositorioPacientes.Received(1).BuscarPacientePorCuil(cuil);
    }

    [Fact]
    public void BuscarPacientePorCuil_PacienteNoExiste_RetornaNull()
    {
        // Arrange
        string cuil = "20-30123456-3";
        _repositorioPacientes.BuscarPacientePorCuil(cuil).Returns((Paciente?)null);

        // Act
        var resultado = _servicioPacientes.BuscarPacientePorCuil(cuil);

        // Assert
        resultado.Should().BeNull();
        _repositorioPacientes.Received(1).BuscarPacientePorCuil(cuil);
    }
}