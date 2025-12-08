using Dominio.Validadores;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTests.Dominio.Validadores;

public class ValidadorEmailTests
{
    [Theory]
    [InlineData("usuario@ejemplo.com")]
    [InlineData("test.user@dominio.com.ar")]
    [InlineData("admin@hospital.gov")]
    [InlineData("enfermera123@clinica.org")]
    [InlineData("doctor_medico@urgencias.net")]
    public void EsValido_ConEmailValido_RetornaTrue(string email)
    {
        // Act
        var resultado = ValidadorEmail.EsValido(email);

        // Assert
        resultado.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]               // Vacío
    [InlineData(" ")]              // Solo espacios
    [InlineData("invalido")]       // Sin @
    [InlineData("@ejemplo.com")]   // Sin parte local
    [InlineData("usuario@")]       // Sin dominio
    [InlineData("usuario @ejemplo.com")]  // Espacio en parte local
    [InlineData("usuario@ejemplo .com")]  // Espacio en dominio
    public void EsValido_ConEmailInvalido_RetornaFalse(string email)
    {
        // Act
        var resultado = ValidadorEmail.EsValido(email);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void EsValido_ConEmailNull_RetornaFalse()
    {
        // Act
        var resultado = ValidadorEmail.EsValido(null);

        // Assert
        resultado.Should().BeFalse();
    }
}