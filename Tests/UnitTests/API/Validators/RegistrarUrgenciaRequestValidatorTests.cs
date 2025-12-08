using FluentAssertions;
using API.DTOs.Urgencias;
using API.Validators;
using Xunit;
using Dominio.Entidades;

namespace Tests.UnitTests.Web.Validators;

public class RegistrarUrgenciaRequestValidatorTests
{
    private readonly RegistrarUrgenciaRequestValidator _validator;

    public RegistrarUrgenciaRequestValidatorTests()
    {
        _validator = new RegistrarUrgenciaRequestValidator();
    }

    [Fact]
    public void Validate_ConDatosValidos_EsValido()
    {
        // Arrange
        var request = new RegistrarUrgenciaRequest
        {
            CuilPaciente = "20-30123456-3",
            Informe = "Dolor de cabeza intenso",
            Temperatura = 37.5,
            NivelEmergencia = NivelEmergencia.URGENCIA,
            FrecuenciaCardiaca = 80,
            FrecuenciaRespiratoria = 18,
            FrecuenciaSistolica = 120,
            FrecuenciaDiastolica = 80
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeTrue();
        resultado.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_SinCUIL_NoEsValido(string cuilInvalido)
    {
        // Arrange
        var request = new RegistrarUrgenciaRequest
        {
            CuilPaciente = cuilInvalido,
            Informe = "Informe válido",
            Temperatura = 37.0,
            NivelEmergencia = NivelEmergencia.URGENCIA,
            FrecuenciaCardiaca = 80,
            FrecuenciaRespiratoria = 18,
            FrecuenciaSistolica = 120,
            FrecuenciaDiastolica = 80
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "CuilPaciente");
    }

    [Theory]
    [InlineData("20-30123456-9")] // Dígito verificador incorrecto
    [InlineData("20301234569")]    // Sin guiones, dígito verificador incorrecto
    [InlineData("123456")]          // Muy corto
    [InlineData("ABCDEFGHIJK")]     // Letras
    public void Validate_ConCUILInvalido_NoEsValido(string cuilInvalido)
    {
        // Arrange
        var request = new RegistrarUrgenciaRequest
        {
            CuilPaciente = cuilInvalido,
            Informe = "Informe válido",
            Temperatura = 37.0,
            NivelEmergencia = NivelEmergencia.URGENCIA,
            FrecuenciaCardiaca = 80,
            FrecuenciaRespiratoria = 18,
            FrecuenciaSistolica = 120,
            FrecuenciaDiastolica = 80
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "CuilPaciente");
    }

    [Theory]
    [InlineData("20-30123456-3")] // Con guiones
    [InlineData("20301234563")]    // Sin guiones
    public void Validate_ConCUILValido_EsValido(string cuilValido)
    {
        // Arrange
        var request = new RegistrarUrgenciaRequest
        {
            CuilPaciente = cuilValido,
            Informe = "Informe válido",
            Temperatura = 37.0,
            NivelEmergencia = NivelEmergencia.URGENCIA,
            FrecuenciaCardiaca = 80,
            FrecuenciaRespiratoria = 18,
            FrecuenciaSistolica = 120,
            FrecuenciaDiastolica = 80
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_SinInforme_NoEsValido(string informeInvalido)
    {
        // Arrange
        var request = new RegistrarUrgenciaRequest
        {
            CuilPaciente = "20-30123456-3",
            Informe = informeInvalido,
            Temperatura = 37.0,
            NivelEmergencia = NivelEmergencia.URGENCIA,
            FrecuenciaCardiaca = 80,
            FrecuenciaRespiratoria = 18,
            FrecuenciaSistolica = 120,
            FrecuenciaDiastolica = 80
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Informe");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(60)] // Temperatura fuera de rango razonable
    public void Validate_ConTemperaturaInvalida_NoEsValido(double temperaturaInvalida)
    {
        // Arrange
        var request = new RegistrarUrgenciaRequest
        {
            CuilPaciente = "20-30123456-3",
            Informe = "Informe válido",
            Temperatura = temperaturaInvalida,
            NivelEmergencia = NivelEmergencia.URGENCIA,
            FrecuenciaCardiaca = 80,
            FrecuenciaRespiratoria = 18,
            FrecuenciaSistolica = 120,
            FrecuenciaDiastolica = 80
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Temperatura");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Validate_ConFrecuenciaCardiacaInvalida_NoEsValido(double frecuenciaInvalida)
    {
        // Arrange
        var request = new RegistrarUrgenciaRequest
        {
            CuilPaciente = "20-30123456-3",
            Informe = "Informe válido",
            Temperatura = 37.0,
            NivelEmergencia = NivelEmergencia.URGENCIA,
            FrecuenciaCardiaca = frecuenciaInvalida,
            FrecuenciaRespiratoria = 18,
            FrecuenciaSistolica = 120,
            FrecuenciaDiastolica = 80
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "FrecuenciaCardiaca");
    }
}