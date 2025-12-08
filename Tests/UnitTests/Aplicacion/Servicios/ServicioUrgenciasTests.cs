using Aplicacion;
using Aplicacion.Servicios;
using Dominio.Entidades;
using Dominio.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Tests.UnitTests.Aplicacion.Servicios;

public class ServicioUrgenciasTests
{
    private readonly IRepositorioPacientes _repositorioPacientes;
    private readonly IRepositorioUrgencias _repositorioUrgencias;
    private readonly ServicioUrgencias _servicioUrgencias;
    private readonly Enfermera _enfermera;

    public ServicioUrgenciasTests()
    {
        _repositorioPacientes = Substitute.For<IRepositorioPacientes>();
        _repositorioUrgencias = Substitute.For<IRepositorioUrgencias>();
        _servicioUrgencias = new ServicioUrgencias(_repositorioPacientes, _repositorioUrgencias);

        _enfermera = new Enfermera
        {
            Nombre = "María",
            Apellido = "González",
            Matricula = "ENF001"
        };
    }

    #region RegistrarUrgencia Tests

    [Fact]
    public void RegistrarUrgencia_ConPacienteExistente_RegistraIngreso()
    {
        // Arrange
        string cuil = "20-30123456-3";
        var pacienteExistente = new Paciente
        {
            CUIL = cuil,
            DNI = 30123456,
            Nombre = "Juan",
            Apellido = "Pérez",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Domicilio = new Domicilio { Calle = "San Martín", Numero = 123, Localidad = "Tucumán" }
        };

        _repositorioPacientes.BuscarPacientePorCuil(cuil).Returns(pacienteExistente);

        // Act
        _servicioUrgencias.RegistrarUrgencia(
            cuil, _enfermera, "Dolor de pecho", 38.0, NivelEmergencia.CRITICA,
            95, 22, 140, 90
        );

        // Assert
        _repositorioPacientes.Received(1).BuscarPacientePorCuil(cuil);
        _repositorioPacientes.DidNotReceive().RegistrarPaciente(Arg.Any<Paciente>());
        _repositorioUrgencias.Received(1).AgregarIngreso(Arg.Is<Ingreso>(i =>
            i.Paciente == pacienteExistente &&
            i.NivelEmergencia == NivelEmergencia.CRITICA &&
            i.Estado == EstadoIngreso.PENDIENTE
        ));
    }

    [Fact]
    public void RegistrarUrgencia_ConPacienteNuevo_CreaPacienteGenericoYRegistraIngreso()
    {
        // Arrange
        string cuil = "20-30123456-3";
        _repositorioPacientes.BuscarPacientePorCuil(cuil).Returns((Paciente?)null);

        _repositorioPacientes.RegistrarPaciente(Arg.Any<Paciente>())
            .Returns(devuelve => devuelve.Arg<Paciente>());

        // Act
        _servicioUrgencias.RegistrarUrgencia(
            cuil, _enfermera, "Consulta", 36.8, NivelEmergencia.URGENCIA_MENOR,
            75, 16, 120, 80
        );

        // Assert
        _repositorioPacientes.Received(1).BuscarPacientePorCuil(cuil);
        _repositorioPacientes.Received(1).RegistrarPaciente(Arg.Is<Paciente>(p =>
            p.CUIL == cuil &&
            p.Nombre == "Sin Registrar" &&
            p.Apellido == "Sin Registrar" &&
            p.Domicilio.Calle == "Sin Registrar"
        ));
        _repositorioUrgencias.Received(1).AgregarIngreso(Arg.Any<Ingreso>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RegistrarUrgencia_SinInforme_LanzaExcepcion(string informeInvalido)
    {
        // Arrange
        string cuil = "20-30123456-3";

        // Act
        Action act = () => _servicioUrgencias.RegistrarUrgencia(
            cuil, _enfermera, informeInvalido, 37.0, NivelEmergencia.URGENCIA,
            80, 18, 120, 80
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El informe es un dato mandatorio");
    }

    [Fact]
    public void RegistrarUrgencia_ConSignosVitalesInvalidos_LanzaExcepcion()
    {
        // Arrange
        string cuil = "20-30123456-3";

        // Act
        Action act = () => _servicioUrgencias.RegistrarUrgencia(
            cuil, _enfermera, "Informe", 37.0, NivelEmergencia.URGENCIA,
            -10, // Frecuencia cardíaca negativa
            18, 120, 80
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*frecuencia cardiaca*");
    }

    #endregion

    #region ReclamarPaciente Tests

    [Fact]
    public void ReclamarPaciente_ConPacienteEnCola_CambiaEstadoAEnProceso()
    {
        // Arrange
        var doctor = new Doctor { Nombre = "Dr. Juan", Apellido = "Pérez", Matricula = "MP001" };
        var paciente = new Paciente
        {
            CUIL = "20-30123456-3",
            DNI = 30123456,
            Nombre = "Juan",
            Apellido = "Pérez",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Domicilio = new Domicilio { Calle = "Calle", Numero = 123, Localidad = "Tucumán" }
        };

        var ingresoPendiente = new Ingreso(
            paciente, _enfermera, "Dolor de cabeza", NivelEmergencia.URGENCIA,
            37.0, 80, 18, 120, 80
        );

        _repositorioUrgencias.ObtenerIngresosPendientes()
            .Returns(new List<Ingreso> { ingresoPendiente });

        // Act
        var resultado = _servicioUrgencias.ReclamarPaciente(doctor);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Estado.Should().Be(EstadoIngreso.EN_PROCESO);
        _repositorioUrgencias.Received(1).ObtenerIngresosPendientes();
        _repositorioUrgencias.Received(1).ActualizarIngreso(Arg.Is<Ingreso>(i =>
            i.Estado == EstadoIngreso.EN_PROCESO
        ));
    }

    [Fact]
    public void ReclamarPaciente_SinPacientesEnCola_LanzaExcepcion()
    {
        // Arrange
        var doctor = new Doctor { Nombre = "Dr. Juan", Apellido = "Pérez", Matricula = "MP001" };
        _repositorioUrgencias.ObtenerIngresosPendientes()
            .Returns(new List<Ingreso>());

        // Act
        Action act = () => _servicioUrgencias.ReclamarPaciente(doctor);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No hay pacientes en la lista de espera");
        _repositorioUrgencias.DidNotReceive().ActualizarIngreso(Arg.Any<Ingreso>());
    }

    [Fact]
    public void ReclamarPaciente_ConDoctorNull_LanzaExcepcion()
    {
        // Act
        Action act = () => _servicioUrgencias.ReclamarPaciente(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*doctor*");
    }

    #endregion

    #region CancelarAtencion Tests

    [Fact]
    public void CancelarAtencion_ConIngresoEnProceso_RevierteToPendiente()
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
            Domicilio = new Domicilio { Calle = "Calle", Numero = 123, Localidad = "Tucumán" }
        };

        var ingresoEnProceso = new Ingreso(
            paciente, _enfermera, "Consulta", NivelEmergencia.URGENCIA,
            37.0, 80, 18, 120, 80
        );
        ingresoEnProceso.Estado = EstadoIngreso.EN_PROCESO;

        _repositorioUrgencias.BuscarIngresoPorCuilYEstado(cuil, EstadoIngreso.EN_PROCESO)
            .Returns(ingresoEnProceso);

        // Act
        _servicioUrgencias.CancelarAtencion(cuil);

        // Assert
        _repositorioUrgencias.Received(1).BuscarIngresoPorCuilYEstado(cuil, EstadoIngreso.EN_PROCESO);
        _repositorioUrgencias.Received(1).ActualizarIngreso(Arg.Is<Ingreso>(i =>
            i.Estado == EstadoIngreso.PENDIENTE
        ));
    }

    [Fact]
    public void CancelarAtencion_SinIngresoEnProceso_LanzaExcepcion()
    {
        // Arrange
        string cuil = "20-30123456-3";
        _repositorioUrgencias.BuscarIngresoPorCuilYEstado(cuil, EstadoIngreso.EN_PROCESO)
            .Returns((Ingreso?)null);

        // Act
        Action act = () => _servicioUrgencias.CancelarAtencion(cuil);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*no se encontró un ingreso en proceso*");
    }

    #endregion

    #region ObtenerIngresosPendientes Tests

    [Fact]
    public void ObtenerIngresosPendientes_DelegaAlRepositorio()
    {
        // Arrange
        var ingresos = new List<Ingreso>();
        _repositorioUrgencias.ObtenerIngresosPendientes().Returns(ingresos);

        // Act
        var resultado = _servicioUrgencias.ObtenerIngresosPendientes();

        // Assert
        resultado.Should().BeSameAs(ingresos);
        _repositorioUrgencias.Received(1).ObtenerIngresosPendientes();
    }

    #endregion
}