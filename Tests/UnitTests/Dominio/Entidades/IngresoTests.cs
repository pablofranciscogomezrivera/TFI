using Dominio.Entidades;
using Dominio.Enums;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTests.Dominio.Entidades;

public class IngresoTests
{
    private readonly Paciente _paciente;
    private readonly Enfermera _enfermera;

    public IngresoTests()
    {
        _paciente = new Paciente
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

        _enfermera = new Enfermera
        {
            Nombre = "María",
            Apellido = "González",
            Matricula = "ENF001"
        };
    }

    [Fact]
    public void CompareTo_IngresoCriticoVsUrgencia_CriticoEsPrimero()
    {
        // Arrange
        var ingresoCritico = new Ingreso(
            _paciente, _enfermera, "Infarto", NivelEmergencia.CRITICA,
            38.0, 120, 25, 180, 110
        );

        var ingresoUrgencia = new Ingreso(
            _paciente, _enfermera, "Fractura", NivelEmergencia.URGENCIA,
            37.0, 80, 18, 120, 80
        );

        // Act
        var resultado = ingresoCritico.CompareTo(ingresoUrgencia);

        // Assert
        resultado.Should().BeLessThan(0); // Crítico debe ir antes (menor)
    }

    [Fact]
    public void CompareTo_MismaPrioridad_PrimeroQueLlegaPrimero()
    {
        // Arrange
        var primerIngreso = new Ingreso(
            _paciente, _enfermera, "Primero", NivelEmergencia.URGENCIA,
            37.0, 80, 18, 120, 80
        );

        System.Threading.Thread.Sleep(10); // Asegurar diferente timestamp

        var segundoIngreso = new Ingreso(
            _paciente, _enfermera, "Segundo", NivelEmergencia.URGENCIA,
            37.0, 80, 18, 120, 80
        );

        // Act
        var resultado = primerIngreso.CompareTo(segundoIngreso);

        // Assert
        resultado.Should().BeLessThan(0); // El primero debe ir antes
    }

    [Fact]
    public void CompareTo_ConIngresoNull_RetornaPositivo()
    {
        // Arrange
        var ingreso = new Ingreso(
            _paciente, _enfermera, "Test", NivelEmergencia.URGENCIA,
            37.0, 80, 18, 120, 80
        );

        // Act
        var resultado = ingreso.CompareTo(null);

        // Assert
        resultado.Should().BePositive(); // No null es "mayor" que null
    }

    [Fact]
    public void Constructor_CreaIngresoConEstadoPendiente()
    {
        // Act
        var ingreso = new Ingreso(
            _paciente, _enfermera, "Test", NivelEmergencia.URGENCIA,
            37.0, 80, 18, 120, 80
        );

        // Assert
        ingreso.Estado.Should().Be(EstadoIngreso.PENDIENTE);
        ingreso.Atencion.Should().BeNull();
        ingreso.FechaIngreso.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(NivelEmergencia.CRITICA, NivelEmergencia.EMERGENCIA, -1)]
    [InlineData(NivelEmergencia.EMERGENCIA, NivelEmergencia.URGENCIA, -1)]
    [InlineData(NivelEmergencia.URGENCIA, NivelEmergencia.URGENCIA_MENOR, -1)]
    [InlineData(NivelEmergencia.URGENCIA_MENOR, NivelEmergencia.SIN_URGENCIA, -1)]
    public void CompareTo_VerificaOrdenCompletoDePrioridades(
        NivelEmergencia nivelMayorPrioridad,
        NivelEmergencia nivelMenorPrioridad,
        int resultadoEsperado)
    {
        // Arrange
        var ingresoMayor = new Ingreso(
            _paciente, _enfermera, "Mayor prioridad", nivelMayorPrioridad,
            37.0, 80, 18, 120, 80
        );

        var ingresoMenor = new Ingreso(
            _paciente, _enfermera, "Menor prioridad", nivelMenorPrioridad,
            37.0, 80, 18, 120, 80
        );

        // Act
        var resultado = Math.Sign(ingresoMayor.CompareTo(ingresoMenor));

        // Assert
        resultado.Should().Be(resultadoEsperado);
    }
}