using Dominio.Validadores;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTests;

public class ValidadorEmailTests
{
    [Theory]
    [InlineData("usuario@ejemplo.com")]
    [InlineData("test.user@dominio.com.ar")]
    [InlineData("admin@hospital.gov")]
    [InlineData("enfermera123@clinica.org")]
    public void EsValido_EmailValido_RetornaTrue(string email)
    {
        // Act
        var resultado = ValidadorEmail.EsValido(email);

        // Assert
        resultado.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalido")]
    [InlineData("@ejemplo.com")]
    [InlineData("usuario@")]
    [InlineData("usuario @ejemplo.com")]
    [InlineData("usuario@ejemplo .com")]
    public void EsValido_EmailInvalido_RetornaFalse(string email)
    {
        // Act
        var resultado = ValidadorEmail.EsValido(email);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void EsValido_EmailNull_RetornaFalse()
    {
        // Act
        var resultado = ValidadorEmail.EsValido(null);

        // Assert
        resultado.Should().BeFalse();
    }
}
