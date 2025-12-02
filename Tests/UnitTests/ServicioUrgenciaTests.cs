using Aplicacion;
using Dominio.Entidades;
using FluentAssertions;
using Infraestructura.Memory;
using Xunit;

namespace Tests.UnitTests;

public class ServicioUrgenciasTests
{
    private readonly ServicioUrgencias _servicioUrgencias;
    private readonly RepositorioPacientesMemoria _repositorioPacientes;
    private readonly RepositorioUrgenciasMemoria _repositorioUrgencias;
    private readonly Enfermera _enfermera;

    public ServicioUrgenciasTests()
    {
        _repositorioPacientes = new RepositorioPacientesMemoria();
        _repositorioUrgencias = new RepositorioUrgenciasMemoria();
        _servicioUrgencias = new ServicioUrgencias(_repositorioPacientes, _repositorioUrgencias);

        _enfermera = new Enfermera
        {
            Nombre = "Enfermera",
            Apellido = "González",
            Matricula = "ENF123"
        };
    }

    #region IS2025-001: Tests de Registro de Urgencias

    [Fact]
    public void RegistrarUrgencia_ConPacienteExistente_RegistroExitoso()
    {
        // Arrange - Registrar paciente primero
        var paciente = new Paciente
        {
            CUIL = "20-30123456-3",
            DNI = 30123456,
            Nombre = "Juan",
            Apellido = "Pérez",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Domicilio = new Domicilio
            {
                Calle = "San Martín",
                Numero = 123,
                Localidad = "Tucumán"
            }
        };
        _repositorioPacientes.RegistrarPaciente(paciente);

        // Act - Registrar urgencia
        _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Dolor de pecho",
            38.0,
            NivelEmergencia.CRITICA,
            95,
            22,
            140,
            90
        );

        // Assert
        var ingresos = _servicioUrgencias.ObtenerIngresosPendientes();
        ingresos.Should().HaveCount(1);
        ingresos[0].Paciente.CUIL.Should().Be("20-30123456-3");
        ingresos[0].NivelEmergencia.Should().Be(NivelEmergencia.CRITICA);
        ingresos[0].Estado.Should().Be(EstadoIngreso.PENDIENTE);
    }

    //[Fact]
    //public void RegistrarUrgencia_ConPacienteNuevoConDatos_CreaPacienteYRegistraUrgencia()
    //{
    //    // Arrange - Paciente NO existe en el sistema
    //    string cuil = "20-30123456-3";

    //    // Act - Registrar urgencia con datos del paciente
    //    _servicioUrgencias.RegistrarUrgencia(
    //        cuil,
    //        _enfermera,
    //        "Fiebre alta",
    //        39.0,
    //        NivelEmergencia.URGENCIA,
    //        85,
    //        20,
    //        130,
    //        85,
    //        // Datos del paciente nuevo
    //        "María",
    //        "García",
    //        "Belgrano",
    //        456,
    //        "Yerba Buena",
    //        new DateTime(1985, 5, 15)
    //    );

    //    // Assert - Verificar que el paciente fue creado
    //    var paciente = _repositorioPacientes.BuscarPacientePorCuil(cuil);
    //    paciente.Should().NotBeNull();
    //    paciente!.Nombre.Should().Be("María");
    //    paciente.Apellido.Should().Be("García");
    //    paciente.Domicilio.Calle.Should().Be("Belgrano");
    //    paciente.Domicilio.Numero.Should().Be(456);
    //    paciente.Domicilio.Localidad.Should().Be("Yerba Buena");

    //    // Verificar que la urgencia fue registrada
    //    var ingresos = _servicioUrgencias.ObtenerIngresosPendientes();
    //    ingresos.Should().HaveCount(1);
    //    ingresos[0].Paciente.Should().Be(paciente);
    //    ingresos[0].NivelEmergencia.Should().Be(NivelEmergencia.URGENCIA);
    //}

    [Fact]
    public void RegistrarUrgencia_ConPacienteNuevoSinDatos_CreaPacienteConDatosGenericos()
    {
        // Arrange - Paciente NO existe en el sistema
        string cuil = "20-30123456-3";

        // Act - Registrar urgencia SIN datos del paciente (usa valores por defecto)
        _servicioUrgencias.RegistrarUrgencia(
            cuil,
            _enfermera,
            "Consulta general",
            36.8,
            NivelEmergencia.URGENCIA_MENOR,
            75,
            16,
            120,
            80
        );

        // Assert - Verificar que el paciente fue creado con datos genéricos
        var paciente = _repositorioPacientes.BuscarPacientePorCuil(cuil);
        paciente.Should().NotBeNull();
        paciente!.Nombre.Should().Be("Sin Registrar");
        paciente.Apellido.Should().Be("Sin Registrar");
        paciente.Domicilio.Calle.Should().Be("Sin Registrar");
        paciente.Domicilio.Numero.Should().Be(999);
        paciente.Domicilio.Localidad.Should().Be("San Miguel de Tucumán");

        // Verificar que la urgencia fue registrada
        var ingresos = _servicioUrgencias.ObtenerIngresosPendientes();
        ingresos.Should().HaveCount(1);
        ingresos[0].Paciente.Should().Be(paciente);
    }

    [Fact]
    public void RegistrarUrgencia_SinInforme_LanzaExcepcion()
    {
        // Arrange
        var paciente = new Paciente
        {
            CUIL = "20-30123456-3",
            DNI = 30123456,
            Nombre = "Juan",
            Apellido = "Pérez",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Domicilio = new Domicilio { Calle = "Calle", Numero = 123, Localidad = "Tucumán" }
        };
        _repositorioPacientes.RegistrarPaciente(paciente);

        // Act
        Action act = () => _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "", // Informe vacío
            37.0,
            NivelEmergencia.URGENCIA,
            80,
            18,
            120,
            80
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El informe es un dato mandatorio");
    }

    [Fact]
    public void RegistrarUrgencia_ConFrecuenciaCardiaNegativa_LanzaExcepcion()
    {
        // Arrange
        var paciente = new Paciente
        {
            CUIL = "20-30123456-3",
            DNI = 30123456,
            Nombre = "Juan",
            Apellido = "Pérez",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Domicilio = new Domicilio { Calle = "Calle", Numero = 123, Localidad = "Tucumán" }
        };
        _repositorioPacientes.RegistrarPaciente(paciente);

        // Act
        Action act = () => _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Dolor de cabeza",
            37.0,
            NivelEmergencia.URGENCIA,
            -10, // Frecuencia cardíaca negativa
            18,
            120,
            80
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("La frecuencia cardiaca no puede ser negativa");
    }

    [Fact]
    public void RegistrarUrgencia_ConFrecuenciaRespiratoriaNegativa_LanzaExcepcion()
    {
        // Arrange
        var paciente = new Paciente
        {
            CUIL = "20-30123456-3",
            DNI = 30123456,
            Nombre = "Juan",
            Apellido = "Pérez",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Domicilio = new Domicilio { Calle = "Calle", Numero = 123, Localidad = "Tucumán" }
        };
        _repositorioPacientes.RegistrarPaciente(paciente);

        // Act
        Action act = () => _servicioUrgencias.RegistrarUrgencia(
            "20-30123456-3",
            _enfermera,
            "Dolor de cabeza",
            37.0,
            NivelEmergencia.URGENCIA,
            80,
            -5, // Frecuencia respiratoria negativa
            120,
            80
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("La frecuencia respiratoria no puede ser negativa");
    }

    #endregion

    #region Tests de Orden de Prioridad

    [Fact]
    public void RegistrarUrgencias_DiferentesNiveles_MantienePrioridadCorrecta()
    {
        // Arrange - Registrar 3 pacientes
        var paciente1 = new Paciente { CUIL = "20-30123456-3", DNI = 30123456, Nombre = "Paciente", Apellido = "Uno", FechaNacimiento = DateTime.Now, Domicilio = new Domicilio { Calle = "Calle", Numero = 1, Localidad = "Tucumán" } };
        var paciente2 = new Paciente { CUIL = "27-20123456-0", DNI = 20123456, Nombre = "Paciente", Apellido = "Dos", FechaNacimiento = DateTime.Now, Domicilio = new Domicilio { Calle = "Calle", Numero = 2, Localidad = "Tucumán" } };
        var paciente3 = new Paciente { CUIL = "23-25123456-9", DNI = 25123456, Nombre = "Paciente", Apellido = "Tres", FechaNacimiento = DateTime.Now, Domicilio = new Domicilio { Calle = "Calle", Numero = 3, Localidad = "Tucumán" } };

        _repositorioPacientes.RegistrarPaciente(paciente1);
        _repositorioPacientes.RegistrarPaciente(paciente2);
        _repositorioPacientes.RegistrarPaciente(paciente3);

        // Act - Registrar en orden: SIN_URGENCIA, CRITICA, URGENCIA
        _servicioUrgencias.RegistrarUrgencia("20-30123456-3", _enfermera, "Paciente 1", 36.5, NivelEmergencia.SIN_URGENCIA, 70, 15, 110, 70);
        System.Threading.Thread.Sleep(10);
        _servicioUrgencias.RegistrarUrgencia("27-20123456-0", _enfermera, "Paciente 2", 38.5, NivelEmergencia.CRITICA, 120, 25, 180, 110);
        System.Threading.Thread.Sleep(10);
        _servicioUrgencias.RegistrarUrgencia("23-25123456-9", _enfermera, "Paciente 3", 37.5, NivelEmergencia.URGENCIA, 85, 18, 130, 85);

        // Assert - Orden debe ser: CRITICA, URGENCIA, SIN_URGENCIA
        var ingresos = _servicioUrgencias.ObtenerIngresosPendientes();
        ingresos.Should().HaveCount(3);
        ingresos[0].NivelEmergencia.Should().Be(NivelEmergencia.CRITICA);
        ingresos[1].NivelEmergencia.Should().Be(NivelEmergencia.URGENCIA);
        ingresos[2].NivelEmergencia.Should().Be(NivelEmergencia.SIN_URGENCIA);
    }

    [Fact]
    public void RegistrarUrgencias_MismoNivel_OrdenPorFechaIngreso()
    {
        // Arrange
        var paciente1 = new Paciente { CUIL = "20-30123456-3", DNI = 30123456, Nombre = "Primero", Apellido = "Llega", FechaNacimiento = DateTime.Now, Domicilio = new Domicilio { Calle = "Calle", Numero = 1, Localidad = "Tucumán" } };
        var paciente2 = new Paciente { CUIL = "27-20123456-0", DNI = 20123456, Nombre = "Segundo", Apellido = "Llega", FechaNacimiento = DateTime.Now, Domicilio = new Domicilio { Calle = "Calle", Numero = 2, Localidad = "Tucumán" } };

        _repositorioPacientes.RegistrarPaciente(paciente1);
        _repositorioPacientes.RegistrarPaciente(paciente2);

        // Act - Registrar dos pacientes con el mismo nivel
        _servicioUrgencias.RegistrarUrgencia("20-30123456-3", _enfermera, "Primero en llegar", 37.0, NivelEmergencia.URGENCIA, 80, 18, 120, 80);
        System.Threading.Thread.Sleep(50); // Asegurar diferencia de tiempo
        _servicioUrgencias.RegistrarUrgencia("27-20123456-0", _enfermera, "Segundo en llegar", 37.0, NivelEmergencia.URGENCIA, 82, 19, 122, 82);

        // Assert - El primero que llegó debe estar primero
        var ingresos = _servicioUrgencias.ObtenerIngresosPendientes();
        ingresos.Should().HaveCount(2);
        ingresos[0].Paciente.Nombre.Should().Be("Primero");
        ingresos[1].Paciente.Nombre.Should().Be("Segundo");
    }

    #endregion
}
