using Dominio.Validadores;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTests.Dominio.Validadores;

public class ValidadorCUILTests
{
    [Theory]
    [InlineData("20-30123456-3")] // CUIL válido con guiones
    [InlineData("20301234563")]    // CUIL válido sin guiones
    [InlineData("27-20123456-0")]  // CUIL femenino válido
    [InlineData("23-35654321-9")]  // Otro CUIL válido
    public void EsValido_ConCUILValido_RetornaTrue(string cuil)
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
    [InlineData("99-12345678-0")]  // Tipo de CUIL inválido
    public void EsValido_ConCUILInvalido_RetornaFalse(string cuil)
    {
        // Act
        var resultado = ValidadorCUIL.EsValido(cuil);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void EsValido_VerificaAlgoritmoDigitoVerificador()
    {
        // Arrange - CUIL conocido con dígito verificador correcto
        string cuilValido = "20-30123456-3";
        string cuilInvalidoDigito = "20-30123456-5"; // Mismo DNI, dígito incorrecto

        // Act & Assert
        ValidadorCUIL.EsValido(cuilValido).Should().BeTrue();
        ValidadorCUIL.EsValido(cuilInvalidoDigito).Should().BeFalse();
    }
}