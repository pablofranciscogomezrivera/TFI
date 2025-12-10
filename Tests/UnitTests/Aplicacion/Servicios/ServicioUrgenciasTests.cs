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
    private readonly IRepositorioObraSocial _repositorioObraSocial;
    private readonly ServicioUrgencias _servicioUrgencias;
    private readonly Enfermera _enfermera;

    public ServicioUrgenciasTests()
    {
        // Arrange - Mocks con NSubstitute
        _repositorioPacientes = Substitute.For<IRepositorioPacientes>();
        _repositorioUrgencias = Substitute.For<IRepositorioUrgencias>();
        _repositorioObraSocial = Substitute.For<IRepositorioObraSocial>();
        _servicioUrgencias =
            new ServicioUrgencias(_repositorioPacientes, _repositorioUrgencias, _repositorioObraSocial);

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

        // Configurar que RegistrarPaciente devuelva el paciente creado
        _repositorioPacientes.RegistrarPaciente(Arg.Any<Paciente>())
            .Returns(callInfo => callInfo.Arg<Paciente>());

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
    public void ReclamarPaciente_CuandoAsignacionEsExitosaAlPrimerIntento_RetornaIngreso()
    {
        // Arrange
        var doctor = new Doctor { Nombre = "Dr", Apellido = "Pérez", Matricula = "MP001" };
        var candidato = new Ingreso(new Paciente { CUIL = "20-12345678-9" }, _enfermera, "Inf", NivelEmergencia.URGENCIA, 37, 80, 20, 120, 80);

        _repositorioUrgencias.ObtenerSiguienteIngresoPendiente().Returns(candidato);
        _repositorioUrgencias.IntentarAsignarMedico(candidato, doctor).Returns(true);

        // Act
        var resultado = _servicioUrgencias.ReclamarPaciente(doctor);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Estado.Should().Be(EstadoIngreso.EN_PROCESO);
        resultado.Atencion.Doctor.Should().Be(doctor);
        _repositorioUrgencias.Received(1).ObtenerSiguienteIngresoPendiente();
        _repositorioUrgencias.Received(1).IntentarAsignarMedico(candidato, doctor);
    }

    [Fact]
    public void ReclamarPaciente_CuandoPrimerIntentoFalla_ReintentaYRetornaSegundo()
    {
        // Arrange
        var doctor = new Doctor { Matricula = "MP001" };
        var candidato1 = new Ingreso(new Paciente { CUIL = "111" }, _enfermera, "Inf1", NivelEmergencia.URGENCIA, 37, 80, 20, 120, 80);
        var candidato2 = new Ingreso(new Paciente { CUIL = "222" }, _enfermera, "Inf2", NivelEmergencia.URGENCIA, 37, 80, 20, 120, 80);

        _repositorioUrgencias.ObtenerSiguienteIngresoPendiente().Returns(candidato1, candidato2);
        _repositorioUrgencias.IntentarAsignarMedico(candidato1, doctor).Returns(false); // Falló concurrencia
        _repositorioUrgencias.IntentarAsignarMedico(candidato2, doctor).Returns(true);  // Éxito segunda vez

        // Act
        var resultado = _servicioUrgencias.ReclamarPaciente(doctor);

        // Assert
        resultado.Should().BeSameAs(candidato2);
        _repositorioUrgencias.Received(2).ObtenerSiguienteIngresoPendiente();
    }

    [Fact]
    public void ReclamarPaciente_CuandoTodosLosIntentosFallan_LanzaExcepcion()
    {
        // Arrange
        var doctor = new Doctor { Matricula = "MP001" };
        var candidato = new Ingreso(new Paciente { CUIL = "111" }, _enfermera, "Inf", NivelEmergencia.URGENCIA, 37, 80, 20, 120, 80);

        _repositorioUrgencias.ObtenerSiguienteIngresoPendiente().Returns(candidato);
        _repositorioUrgencias.IntentarAsignarMedico(Arg.Any<Ingreso>(), doctor).Returns(false);

        // Act
        Action act = () => _servicioUrgencias.ReclamarPaciente(doctor);

        // Assert
        act.Should().Throw<Exception>()
            .WithMessage("*intente reclamar nuevamente*");

        // 3 Intentos (MaxRetries)
        _repositorioUrgencias.Received(3).ObtenerSiguienteIngresoPendiente();
    }

    [Fact]
    public void ReclamarPaciente_SinPacientesEnCola_LanzaExcepcion()
    {
        // Arrange
        var doctor = new Doctor { Matricula = "MP001" };
        _repositorioUrgencias.ObtenerSiguienteIngresoPendiente().Returns((Ingreso?)null);

        // Act
        Action act = () => _servicioUrgencias.ReclamarPaciente(doctor);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No hay pacientes en la lista de espera");
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