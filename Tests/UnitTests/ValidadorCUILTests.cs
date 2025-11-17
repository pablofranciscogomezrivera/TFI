using Dominio.Validadores;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTests;

public class ValidadorCUILTests
{
    [Theory]
    [InlineData("20-30123456-3")] // CUIL válido con guiones
    [InlineData("20301234563")]    // CUIL válido sin guiones
    [InlineData("27-20123456-0")]  // CUIL femenino válido
    public void EsValido_CUILValido_RetornaTrue(string cuil)
    {
        // Act
        var resultado = ValidadorCUIL.EsValido(cuil);

        // Assert
        resultado.Should().BeTrue();
    }

    [Theory]
    [InlineData("20-30123456-9")]  // Dígito verificador incorrecto
    [InlineData("12345")]          // Longitud incorrecta
    [InlineData("")]               // Vacío
    [InlineData(null)]             // Null
    [InlineData("20-ABCDEFGH-9")]  // Contiene letras
    public void EsValido_CUILInvalido_RetornaFalse(string cuil)
    {
        // Act
        var resultado = ValidadorCUIL.EsValido(cuil);

        // Assert
        resultado.Should().BeFalse();
    }
}
