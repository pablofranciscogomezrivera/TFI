using Dominio.Entidades.ValueObject;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTests.Dominio.ValueObjects;

public class FrecuenciaTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(60)]
    [InlineData(120)]
    [InlineData(200)]
    public void FrecuenciaCardiaca_ConValorValido_CreaInstancia(double valor)
    {
        // Act
        var frecuencia = new FrecuenciaCardiaca(valor);

        // Assert
        frecuencia.Valor.Should().Be(valor);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-50)]
    [InlineData(-100)]
    public void FrecuenciaCardiaca_ConValorNegativo_LanzaExcepcion(double valorNegativo)
    {
        // Act
        Action act = () => new FrecuenciaCardiaca(valorNegativo);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("La frecuencia cardiaca no puede ser negativa");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(12)]
    [InlineData(30)]
    public void FrecuenciaRespiratoria_ConValorValido_CreaInstancia(double valor)
    {
        // Act
        var frecuencia = new FrecuenciaRespiratoria(valor);

        // Assert
        frecuencia.Valor.Should().Be(valor);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void FrecuenciaRespiratoria_ConValorNegativo_LanzaExcepcion(double valorNegativo)
    {
        // Act
        Action act = () => new FrecuenciaRespiratoria(valorNegativo);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("La frecuencia respiratoria no puede ser negativa");
    }

    [Theory]
    [InlineData(90, 60)]   // Valores normales
    [InlineData(120, 80)]  // Valores normales-altos
    [InlineData(180, 110)] // Valores altos
    public void TensionArterial_ConValoresValidos_CreaInstancia(double sistolica, double diastolica)
    {
        // Act
        var tension = new TensionArterial(sistolica, diastolica);

        // Assert
        tension.FrecuenciaSistolica.Valor.Should().Be(sistolica);
        tension.FrecuenciaDiastolica.Valor.Should().Be(diastolica);
    }

    [Theory]
    [InlineData(-120, 80)]  // Sistólica negativa
    [InlineData(120, -80)]  // Diastólica negativa
    [InlineData(-120, -80)] // Ambas negativas
    public void TensionArterial_ConValoresNegativos_LanzaExcepcion(double sistolica, double diastolica)
    {
        // Act
        Action act = () => new TensionArterial(sistolica, diastolica);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}