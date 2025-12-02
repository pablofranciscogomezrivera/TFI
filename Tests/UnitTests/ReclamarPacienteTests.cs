using Aplicacion;
using Dominio.Entidades;
using Infraestructura;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTests;

public class ReclamarPacienteTests
{
    private readonly ServicioUrgencias _servicioUrgencias;
    private readonly Doctor _doctor;
    private readonly Enfermera _enfermera;

    public ReclamarPacienteTests()
    {
        var repositorioPacientes = new RepositorioPacientesMemoria();
        var repositorioUrgencias = new RepositorioUrgenciasMemoria();
        _servicioUrgencias = new ServicioUrgencias(repositorioPacientes, repositorioUrgencias);

        _doctor = new Doctor
        {
            Nombre = "Dr. Juan",
            Apellido = "Pérez",
            Matricula = "MP12345"
        };

        _enfermera = new Enfermera
        {
            Nombre = "Enfermera",
            Apellido = "González",
            Matricula = "ENF123"
        };
    }

    #region Acceptance Criteria 1: Successful Patient Claim

    [Fact]
    public void ReclamarPaciente_ConPacienteEnListaEspera_ReclamaExitoso()
    {
        // Arrange - Registrar un paciente en urgencias
        string cuil = "20-30123456-3";
        _servicioUrgencias.RegistrarUrgencia(
            cuil,
            _enfermera,
            "Dolor de cabeza intenso",
            37.5,
            NivelEmergencia.URGENCIA,
            80,
            18,
            120,
            80
        );

        // Verificar que hay un paciente en lista de espera
        var listaInicial = _servicioUrgencias.ObtenerIngresosPendientes();
        listaInicial.Should().HaveCount(1);
        listaInicial[0].Estado.Should().Be(EstadoIngreso.PENDIENTE);

        // Act - Doctor reclama el paciente
        var ingresoReclamado = _servicioUrgencias.ReclamarPaciente(_doctor);

        // Assert
        ingresoReclamado.Should().NotBeNull();
        ingresoReclamado.Estado.Should().Be(EstadoIngreso.EN_PROCESO);
        ingresoReclamado.Atencion.Doctor.Should().Be(_doctor);
        ingresoReclamado.Paciente.Should().NotBeNull();
        ingresoReclamado.Paciente.CUIL.Should().Be(cuil);

        // Verificar que el paciente ya no está en la lista de espera
        var listaFinal = _servicioUrgencias.ObtenerIngresosPendientes();
        listaFinal.Should().BeEmpty();
    }

    [Fact]
    public void ReclamarPaciente_CambiEstadoDePendienteAEnProceso()
    {
        // Arrange
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Fiebre alta",
            39.0,
            NivelEmergencia.EMERGENCIA,
            90,
            20,
            130,
            85
        );

        var ingresoAntes = _servicioUrgencias.ObtenerIngresosPendientes()[0];
        ingresoAntes.Estado.Should().Be(EstadoIngreso.PENDIENTE);

        // Act
        var ingresoReclamado = _servicioUrgencias.ReclamarPaciente(_doctor);

        // Assert
        ingresoReclamado.Estado.Should().Be(EstadoIngreso.EN_PROCESO);
        ingresoReclamado.Should().BeSameAs(ingresoAntes);
    }

    [Fact]
    public void ReclamarPaciente_AsignaDoctorALaAtencion()
    {
        // Arrange
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Fractura de brazo",
            36.8,
            NivelEmergencia.URGENCIA,
            75,
            16,
            115,
            75
        );

        var ingresoAntes = _servicioUrgencias.ObtenerIngresosPendientes()[0];
        ingresoAntes.Atencion.Doctor.Should().BeNull();

        // Act
        var ingresoReclamado = _servicioUrgencias.ReclamarPaciente(_doctor);

        // Assert
        ingresoReclamado.Atencion.Doctor.Should().Be(_doctor);
        ingresoReclamado.Atencion.Doctor.Matricula.Should().Be("MP12345");
    }

    [Fact]
    public void ReclamarPaciente_EliminaDeLaListaDeEspera()
    {
        // Arrange
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Dolor de estómago",
            37.0,
            NivelEmergencia.URGENCIA_MENOR,
            70,
            15,
            110,
            70
        );

        _servicioUrgencias.ObtenerIngresosPendientes().Should().HaveCount(1);

        // Act
        _servicioUrgencias.ReclamarPaciente(_doctor);

        // Assert
        _servicioUrgencias.ObtenerIngresosPendientes().Should().BeEmpty();
    }

    #endregion

    #region Acceptance Criteria 2: Empty Waiting List

    [Fact]
    public void ReclamarPaciente_ListaEsperaVacia_LanzaExcepcion()
    {
        // Arrange - Lista de espera vacía (no registrar ningún paciente)
        _servicioUrgencias.ObtenerIngresosPendientes().Should().BeEmpty();

        // Act
        Action act = () => _servicioUrgencias.ReclamarPaciente(_doctor);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No hay pacientes en la lista de espera");
    }

    [Fact]
    public void ReclamarPaciente_DespuesDeReclamarTodos_LanzaExcepcion()
    {
        // Arrange - Registrar y reclamar un paciente
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Consulta general",
            36.5,
            NivelEmergencia.SIN_URGENCIA,
            72,
            16,
            118,
            78
        );

        _servicioUrgencias.ReclamarPaciente(_doctor);

        // Act - Intentar reclamar otro paciente cuando ya no hay
        Action act = () => _servicioUrgencias.ReclamarPaciente(_doctor);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No hay pacientes en la lista de espera");
    }

    #endregion

    #region Priority Order Tests

    [Fact]
    public void ReclamarPaciente_RespetaOrdenDePrioridad_PacienteCriticoAntes()
    {
        // Arrange - Registrar pacientes con diferentes prioridades
        // Paciente 1: SIN_URGENCIA (baja prioridad)
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Dolor leve",
            36.5,
            NivelEmergencia.SIN_URGENCIA,
            70,
            15,
            110,
            70
        );

        // Esperar un momento para asegurar diferentes FechaIngreso
        System.Threading.Thread.Sleep(10);

        // Paciente 2: CRITICA (alta prioridad)
        _servicioUrgencias.RegistrarUrgencia(
            "27-20123456-0",
            _enfermera,
            "Infarto",
            38.0,
            NivelEmergencia.CRITICA,
            120,
            25,
            180,
            110
        );

        // Esperar un momento
        System.Threading.Thread.Sleep(10);

        // Paciente 3: URGENCIA (prioridad media)
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Fractura",
            37.0,
            NivelEmergencia.URGENCIA,
            85,
            18,
            125,
            80
        );

        // Act - Reclamar el primer paciente
        var primerPaciente = _servicioUrgencias.ReclamarPaciente(_doctor);

        // Assert - Debe ser el paciente CRITICO
        primerPaciente.NivelEmergencia.Should().Be(NivelEmergencia.CRITICA);
        primerPaciente.Atencion.Informe.Should().Be("Infarto");
    }

    [Fact]
    public void ReclamarPaciente_MismaPrioridad_RespetaOrdenLlegada()
    {
        // Arrange - Registrar dos pacientes con la misma prioridad
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Primer paciente con urgencia",
            37.5,
            NivelEmergencia.URGENCIA,
            80,
            18,
            120,
            80
        );

        // Esperar para asegurar diferentes FechaIngreso
        System.Threading.Thread.Sleep(50);

        _servicioUrgencias.RegistrarUrgencia(
            "27-20123456-0",
            _enfermera,
            "Segundo paciente con urgencia",
            37.8,
            NivelEmergencia.URGENCIA,
            82,
            19,
            122,
            82
        );

        // Act
        var primerReclamado = _servicioUrgencias.ReclamarPaciente(_doctor);

        // Assert - Debe ser el primer paciente que llegó
        primerReclamado.Atencion.Informe.Should().Be("Primer paciente con urgencia");
    }

    [Fact]
    public void ReclamarPaciente_MultiplesReclamaciones_RespetaPrioridad()
    {
        // Arrange - Registrar 3 pacientes
        _servicioUrgencias.RegistrarUrgencia("20-30123456-3", _enfermera, "EMERGENCIA", 38.0, NivelEmergencia.EMERGENCIA, 90, 20, 130, 85);
        System.Threading.Thread.Sleep(10);
        _servicioUrgencias.RegistrarUrgencia("27-20123456-0", _enfermera, "URGENCIA", 37.5, NivelEmergencia.URGENCIA, 80, 18, 120, 80);
        System.Threading.Thread.Sleep(10);
        _servicioUrgencias.RegistrarUrgencia("20-30123456-3", _enfermera, "URGENCIA_MENOR", 37.0, NivelEmergencia.URGENCIA_MENOR, 75, 16, 115, 75);

        var doctor2 = new Doctor { Nombre = "Dr. Ana", Apellido = "Lopez", Matricula = "MP67890" };
        var doctor3 = new Doctor { Nombre = "Dr. Carlos", Apellido = "Martinez", Matricula = "MP11111" };

        // Act - Reclamar los tres pacientes
        var primero = _servicioUrgencias.ReclamarPaciente(_doctor);
        var segundo = _servicioUrgencias.ReclamarPaciente(doctor2);
        var tercero = _servicioUrgencias.ReclamarPaciente(doctor3);

        // Assert - Verificar el orden correcto
        primero.NivelEmergencia.Should().Be(NivelEmergencia.EMERGENCIA);
        segundo.NivelEmergencia.Should().Be(NivelEmergencia.URGENCIA);
        tercero.NivelEmergencia.Should().Be(NivelEmergencia.URGENCIA_MENOR);

        // Verificar que todos están EN_PROCESO
        primero.Estado.Should().Be(EstadoIngreso.EN_PROCESO);
        segundo.Estado.Should().Be(EstadoIngreso.EN_PROCESO);
        tercero.Estado.Should().Be(EstadoIngreso.EN_PROCESO);

        // Verificar que cada uno tiene su doctor asignado
        primero.Atencion.Doctor.Should().Be(_doctor);
        segundo.Atencion.Doctor.Should().Be(doctor2);
        tercero.Atencion.Doctor.Should().Be(doctor3);

        // Verificar que la lista está vacía
        _servicioUrgencias.ObtenerIngresosPendientes().Should().BeEmpty();
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void ReclamarPaciente_DoctorNull_LanzaExcepcion()
    {
        // Arrange
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Dolor de cabeza",
            37.5,
            NivelEmergencia.URGENCIA,
            80,
            18,
            120,
            80
        );

        // Act
        Action act = () => _servicioUrgencias.ReclamarPaciente(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*doctor*");
    }

    #endregion
}