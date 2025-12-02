using Aplicacion;
using Dominio.Entidades;
using Infraestructura;
using FluentAssertions;
using Xunit;
using Infraestructura.Memory;

namespace Tests.UnitTests;

public class ServicioAtencionTests
{
    private readonly ServicioUrgencias _servicioUrgencias;
    private readonly ServicioAtencion _servicioAtencion;
    private readonly Doctor _doctor;
    private readonly Enfermera _enfermera;

    public ServicioAtencionTests()
    {
        var repositorioPacientes = new RepositorioPacientesMemoria();
        var repositorioUrgencias = new RepositorioUrgenciasMemoria();
        _servicioUrgencias = new ServicioUrgencias(repositorioPacientes, repositorioUrgencias);
        _servicioAtencion = new ServicioAtencion();

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

    #region Acceptance Criteria 1: Successful Attention Registration

    [Fact]
    public void RegistrarAtencion_ConInformeMandatorio_RegistroExitoso()
    {
        // Arrange - Registrar paciente en urgencias y reclamarlo
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Paciente con dolor torácico",
            38.0,
            NivelEmergencia.EMERGENCIA,
            95,
            22,
            140,
            90
        );

        var ingreso = _servicioUrgencias.ReclamarPaciente(_doctor);
        ingreso.Estado.Should().Be(EstadoIngreso.EN_PROCESO);

        string informeMedico = "Diagnóstico: Angina de pecho. Tratamiento: Nitroglicerina sublingual.";

        // Act
        var atencion = _servicioAtencion.RegistrarAtencion(ingreso, informeMedico, _doctor);

        // Assert
        atencion.Should().NotBeNull();
        atencion.Informe.Should().Contain(informeMedico);
        atencion.Doctor.Should().Be(_doctor);
        ingreso.Estado.Should().Be(EstadoIngreso.FINALIZADO);
    }

    [Fact]
    public void RegistrarAtencion_CambiaEstadoAFinalizado()
    {
        // Arrange
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Fiebre alta",
            39.5,
            NivelEmergencia.URGENCIA,
            88,
            20,
            135,
            85
        );

        var ingreso = _servicioUrgencias.ReclamarPaciente(_doctor);
        ingreso.Estado.Should().Be(EstadoIngreso.EN_PROCESO);

        // Act
        _servicioAtencion.RegistrarAtencion(ingreso, "Diagnóstico: Gripe. Tratamiento: Paracetamol.", _doctor);

        // Assert
        ingreso.Estado.Should().Be(EstadoIngreso.FINALIZADO);
    }

    [Fact]
    public void RegistrarAtencion_MantieneDoctorAsignado()
    {
        // Arrange
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Fractura de tobillo",
            37.0,
            NivelEmergencia.URGENCIA,
            75,
            16,
            120,
            80
        );

        var ingreso = _servicioUrgencias.ReclamarPaciente(_doctor);
        ingreso.Atencion.Doctor.Should().Be(_doctor);

        // Act
        var atencion = _servicioAtencion.RegistrarAtencion(
            ingreso,
            "Fractura de tobillo izquierdo. Se inmoviliza y se deriva a traumatología.",
            _doctor
        );

        // Assert
        atencion.Doctor.Should().Be(_doctor);
        ingreso.Atencion.Doctor.Should().Be(_doctor);
    }

    [Fact]
    public void RegistrarAtencion_ConservaInformeEnfermera()
    {
        // Arrange
        string informeEnfermera = "Paciente con dolor abdominal intenso";
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            informeEnfermera,
            37.2,
            NivelEmergencia.URGENCIA,
            82,
            18,
            125,
            82
        );

        var ingreso = _servicioUrgencias.ReclamarPaciente(_doctor);
        string informeMedico = "Diagnóstico: Apendicitis aguda. Derivar a cirugía.";

        // Act
        var atencion = _servicioAtencion.RegistrarAtencion(ingreso, informeMedico, _doctor);

        // Assert
        atencion.Informe.Should().Contain(informeEnfermera);
        atencion.Informe.Should().Contain(informeMedico);
    }

    [Fact]
    public void RegistrarAtencion_ConInformeCompleto_TodosLosDatosCorrectos()
    {
        // Arrange
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Paciente con dificultad respiratoria",
            38.5,
            NivelEmergencia.CRITICA,
            110,
            28,
            150,
            95
        );

        var ingreso = _servicioUrgencias.ReclamarPaciente(_doctor);
        string informeMedico = "Diagnóstico: Crisis asmática severa. Tratamiento: Salbutamol nebulizado + corticoides IV.";

        // Act
        var atencion = _servicioAtencion.RegistrarAtencion(ingreso, informeMedico, _doctor);

        // Assert - Verificar todos los componentes de la atención
        atencion.Should().NotBeNull();
        atencion.Doctor.Should().NotBeNull();
        atencion.Doctor.Matricula.Should().Be("MP12345");
        atencion.Informe.Should().NotBeNullOrEmpty();

        // Verificar el ingreso
        ingreso.Estado.Should().Be(EstadoIngreso.FINALIZADO);
        ingreso.Paciente.Should().NotBeNull();
        ingreso.Enfermera.Should().Be(_enfermera);
    }

    #endregion

    #region Acceptance Criteria 2: Missing Report

    [Fact]
    public void RegistrarAtencion_InformeVacio_LanzaExcepcion()
    {
        // Arrange
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Consulta general",
            36.8,
            NivelEmergencia.SIN_URGENCIA,
            72,
            16,
            118,
            78
        );

        var ingreso = _servicioUrgencias.ReclamarPaciente(_doctor);

        // Act
        Action act = () => _servicioAtencion.RegistrarAtencion(ingreso, "", _doctor);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El informe del paciente es mandatorio");
    }

    [Fact]
    public void RegistrarAtencion_InformeNull_LanzaExcepcion()
    {
        // Arrange
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Dolor de cabeza",
            37.0,
            NivelEmergencia.URGENCIA_MENOR,
            75,
            16,
            115,
            75
        );

        var ingreso = _servicioUrgencias.ReclamarPaciente(_doctor);

        // Act
        Action act = () => _servicioAtencion.RegistrarAtencion(ingreso, null, _doctor);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El informe del paciente es mandatorio");
    }

    [Fact]
    public void RegistrarAtencion_InformeSoloEspacios_LanzaExcepcion()
    {
        // Arrange
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Consulta",
            36.5,
            NivelEmergencia.SIN_URGENCIA,
            70,
            15,
            110,
            70
        );

        var ingreso = _servicioUrgencias.ReclamarPaciente(_doctor);

        // Act
        Action act = () => _servicioAtencion.RegistrarAtencion(ingreso, "   ", _doctor);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El informe del paciente es mandatorio");
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void RegistrarAtencion_IngresoNull_LanzaExcepcion()
    {
        // Act
        Action act = () => _servicioAtencion.RegistrarAtencion(
            null,
            "Informe médico",
            _doctor
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*ingreso*");
    }

    [Fact]
    public void RegistrarAtencion_DoctorNull_LanzaExcepcion()
    {
        // Arrange
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Consulta",
            36.5,
            NivelEmergencia.SIN_URGENCIA,
            70,
            15,
            110,
            70
        );

        var ingreso = _servicioUrgencias.ReclamarPaciente(_doctor);

        // Act
        Action act = () => _servicioAtencion.RegistrarAtencion(
            ingreso,
            "Informe médico",
            null
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*médico*");
    }

    [Fact]
    public void RegistrarAtencion_IngresoNoProcesado_LanzaExcepcion()
    {
        // Arrange - Registrar pero NO reclamar el paciente
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Consulta",
            36.5,
            NivelEmergencia.SIN_URGENCIA,
            70,
            15,
            110,
            70
        );

        var ingresoPendiente = _servicioUrgencias.ObtenerIngresosPendientes()[0];
        ingresoPendiente.Estado.Should().Be(EstadoIngreso.PENDIENTE);

        // Act - Intentar registrar atención sin haber reclamado
        Action act = () => _servicioAtencion.RegistrarAtencion(
            ingresoPendiente,
            "Informe médico",
            _doctor
        );

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Solo se pueden registrar atenciones para ingresos en proceso");
    }

    [Fact]
    public void RegistrarAtencion_IngresoYaFinalizado_LanzaExcepcion()
    {
        // Arrange - Registrar, reclamar y finalizar
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Consulta",
            36.5,
            NivelEmergencia.SIN_URGENCIA,
            70,
            15,
            110,
            70
        );

        var ingreso = _servicioUrgencias.ReclamarPaciente(_doctor);
        _servicioAtencion.RegistrarAtencion(ingreso, "Primera atención", _doctor);
        ingreso.Estado.Should().Be(EstadoIngreso.FINALIZADO);

        // Act - Intentar registrar otra atención en el mismo ingreso
        Action act = () => _servicioAtencion.RegistrarAtencion(
            ingreso,
            "Segunda atención",
            _doctor
        );

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Solo se pueden registrar atenciones para ingresos en proceso");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void FlujoCompleto_RegistroReclamoAtencion_FuncionaCorrectamente()
    {
        // Arrange & Act
        // 1. Enfermera registra urgencia
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Paciente con dolor de pecho y dificultad respiratoria",
            38.2,
            NivelEmergencia.CRITICA,
            105,
            24,
            145,
            92
        );

        var listaEspera = _servicioUrgencias.ObtenerIngresosPendientes();
        listaEspera.Should().HaveCount(1);
        listaEspera[0].Estado.Should().Be(EstadoIngreso.PENDIENTE);

        // 2. Doctor reclama paciente
        var ingreso = _servicioUrgencias.ReclamarPaciente(_doctor);
        ingreso.Estado.Should().Be(EstadoIngreso.EN_PROCESO);
        _servicioUrgencias.ObtenerIngresosPendientes().Should().BeEmpty();

        // 3. Doctor registra atención
        var atencion = _servicioAtencion.RegistrarAtencion(
            ingreso,
            "Diagnóstico: Síndrome coronario agudo. Tratamiento: AAS + Clopidogrel. Derivar a hemodinamia.",
            _doctor
        );

        // Assert - Verificar estado final
        ingreso.Estado.Should().Be(EstadoIngreso.FINALIZADO);
        atencion.Doctor.Should().Be(_doctor);
        atencion.Informe.Should().Contain("dolor de pecho"); // Informe enfermera
        atencion.Informe.Should().Contain("Síndrome coronario agudo"); // Informe médico
    }

    [Fact]
    public void MultiplesAtenciones_DiferentesPacientes_FuncionaCorrectamente()
    {
        // Arrange - Registrar 3 pacientes
        _servicioUrgencias.RegistrarUrgencia("20-30123456-3", _enfermera, "Paciente 1: Fractura", 37.0, NivelEmergencia.URGENCIA, 75, 16, 115, 75);
        _servicioUrgencias.RegistrarUrgencia("27-20123456-0", _enfermera, "Paciente 2: Fiebre", 39.0, NivelEmergencia.URGENCIA_MENOR, 80, 18, 120, 80);
        _servicioUrgencias.RegistrarUrgencia("20-30123456-3", _enfermera, "Paciente 3: Dolor", 36.8, NivelEmergencia.SIN_URGENCIA, 70, 15, 110, 70);

        var doctor2 = new Doctor { Nombre = "Dra. Ana", Apellido = "Lopez", Matricula = "MP67890" };
        var doctor3 = new Doctor { Nombre = "Dr. Carlos", Apellido = "Martinez", Matricula = "MP11111" };

        // Act - Cada doctor reclama y atiende un paciente
        var ingreso1 = _servicioUrgencias.ReclamarPaciente(_doctor);
        var ingreso2 = _servicioUrgencias.ReclamarPaciente(doctor2);
        var ingreso3 = _servicioUrgencias.ReclamarPaciente(doctor3);

        var atencion1 = _servicioAtencion.RegistrarAtencion(ingreso1, "Atención 1", _doctor);
        var atencion2 = _servicioAtencion.RegistrarAtencion(ingreso2, "Atención 2", doctor2);
        var atencion3 = _servicioAtencion.RegistrarAtencion(ingreso3, "Atención 3", doctor3);

        // Assert
        ingreso1.Estado.Should().Be(EstadoIngreso.FINALIZADO);
        ingreso2.Estado.Should().Be(EstadoIngreso.FINALIZADO);
        ingreso3.Estado.Should().Be(EstadoIngreso.FINALIZADO);

        atencion1.Doctor.Should().Be(_doctor);
        atencion2.Doctor.Should().Be(doctor2);
        atencion3.Doctor.Should().Be(doctor3);

        _servicioUrgencias.ObtenerIngresosPendientes().Should().BeEmpty();
    }

    #endregion
}
