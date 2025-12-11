using Aplicacion;
using Aplicacion.Servicios;
using Dominio.Entidades;
using Dominio.Enums;
using Dominio.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Tests.UnitTests.Aplicacion.Servicios;

public class ServicioAtencionTests
{
    private readonly IRepositorioUrgencias _repositorioUrgencias;
    private readonly ServicioAtencion _servicioAtencion;
    private readonly Doctor _doctor;
    private readonly Enfermera _enfermera;
    private readonly Paciente _paciente;

    public ServicioAtencionTests()
    {
        _repositorioUrgencias = Substitute.For<IRepositorioUrgencias>();
        _servicioAtencion = new ServicioAtencion(_repositorioUrgencias);

        _doctor = new Doctor { Nombre = "Dr. Juan", Apellido = "Pérez", Matricula = "MP001" };
        _enfermera = new Enfermera { Nombre = "María", Apellido = "González", Matricula = "ENF001" };
        _paciente = new Paciente
        {
            CUIL = "20-30123456-3",
            DNI = 30123456,
            Nombre = "Juan",
            Apellido = "Pérez",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Domicilio = new Domicilio { Calle = "San Martín", Numero = 123, Localidad = "Tucumán" }
        };
    }

    [Fact]
    public void RegistrarAtencion_ConIngresoEnProceso_FinalizaYActualiza()
    {
        // Arrange
        var ingreso = new Ingreso(
            _paciente, _enfermera, "Dolor de pecho", NivelEmergencia.CRITICA,
            38.0, 95, 22, 140, 90
        );
        ingreso.Estado = EstadoIngreso.EN_PROCESO;

        string informeMedico = "Diagnóstico: Angina. Tratamiento: Nitroglicerina.";

        // Act
        var resultado = _servicioAtencion.RegistrarAtencion(ingreso, informeMedico, _doctor);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Doctor.Should().Be(_doctor);
        resultado.Informe.Should().Contain(informeMedico);
        ingreso.Estado.Should().Be(EstadoIngreso.FINALIZADO);
        ingreso.Atencion.Should().Be(resultado);
        _repositorioUrgencias.Received(1).ActualizarIngreso(ingreso);
    }

    [Fact]
    public void RegistrarAtencion_ConIngresoEnEstadoPendiente_LanzaExcepcion()
    {
        // Arrange
        var ingreso = new Ingreso(
            _paciente, _enfermera, "Consulta", NivelEmergencia.URGENCIA,
            37.0, 80, 18, 120, 80
        );
        // Estado = PENDIENTE por defecto

        // Act
        Action act = () => _servicioAtencion.RegistrarAtencion(ingreso, "Informe", _doctor);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Solo se pueden registrar atenciones para ingresos en proceso*");
        _repositorioUrgencias.DidNotReceive().ActualizarIngreso(Arg.Any<Ingreso>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RegistrarAtencion_SinInformeMedico_LanzaExcepcion(string informeInvalido)
    {
        // Arrange
        var ingreso = new Ingreso(
            _paciente, _enfermera, "Consulta", NivelEmergencia.URGENCIA,
            37.0, 80, 18, 120, 80
        );
        ingreso.Estado = EstadoIngreso.EN_PROCESO;

        // Act
        Action act = () => _servicioAtencion.RegistrarAtencion(ingreso, informeInvalido, _doctor);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El informe médico es mandatorio");
    }

    [Fact]
    public void RegistrarAtencion_ConIngresoNull_LanzaExcepcion()
    {
        // Act
        Action act = () => _servicioAtencion.RegistrarAtencion(null, "Informe", _doctor);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*ingreso*");
    }

    [Fact]
    public void RegistrarAtencion_ConDoctorNull_LanzaExcepcion()
    {
        // Arrange
        var ingreso = new Ingreso(
            _paciente, _enfermera, "Consulta", NivelEmergencia.URGENCIA,
            37.0, 80, 18, 120, 80
        );
        ingreso.Estado = EstadoIngreso.EN_PROCESO;

        // Act
        Action act = () => _servicioAtencion.RegistrarAtencion(ingreso, "Informe", null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*médico*");
    }

    [Fact]
    public void RegistrarAtencionPorCuil_ConIngresoEnProceso_RegistraAtencion()
    {
        // Arrange
        string cuil = "20-30123456-3";
        var ingreso = new Ingreso(
            _paciente, _enfermera, "Dolor de cabeza", NivelEmergencia.URGENCIA,
            37.0, 80, 18, 120, 80
        );
        ingreso.Estado = EstadoIngreso.EN_PROCESO;

        _repositorioUrgencias.BuscarIngresoPorCuilYEstado(cuil, EstadoIngreso.EN_PROCESO)
            .Returns(ingreso);

        string informeMedico = "Diagnóstico: Migraña.";

        // Act
        var resultado = _servicioAtencion.RegistrarAtencionPorCuil(cuil, informeMedico, _doctor);

        // Assert
        resultado.Should().NotBeNull();
        ingreso.Estado.Should().Be(EstadoIngreso.FINALIZADO);
        _repositorioUrgencias.Received(1).BuscarIngresoPorCuilYEstado(cuil, EstadoIngreso.EN_PROCESO);
        _repositorioUrgencias.Received(1).ActualizarIngreso(ingreso);
    }

    [Fact]
    public void RegistrarAtencionPorCuil_SinIngresoEnProceso_LanzaExcepcion()
    {
        // Arrange
        string cuil = "20-30123456-3";
        _repositorioUrgencias.BuscarIngresoPorCuilYEstado(cuil, EstadoIngreso.EN_PROCESO)
            .Returns((Ingreso?)null);

        // Act
        Action act = () => _servicioAtencion.RegistrarAtencionPorCuil(cuil, "Informe", _doctor);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*No se encontró un ingreso en proceso*");
    }
}