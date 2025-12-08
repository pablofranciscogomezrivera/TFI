using Dominio.Enums;
using FluentAssertions;
using API.DTOs.Auth;
using API.Validators;
using Xunit;

namespace Tests.UnitTests.Web.Validators;

public class RegistrarUsuarioRequestValidatorTests
{
    private readonly RegistrarUsuarioRequestValidator _validator;

    public RegistrarUsuarioRequestValidatorTests()
    {
        _validator = new RegistrarUsuarioRequestValidator();
    }

    [Fact]
    public void Validate_ConDatosValidos_EsValido()
    {
        // Arrange
        var request = new RegistrarUsuarioRequest
        {
            Email = "doctor@hospital.com",
            Password = "Password123!",
            TipoAutoridad = TipoAutoridad.Medico,
            Nombre = "Juan",
            Apellido = "Pérez",
            DNI = 30123456,
            CUIL = "20-30123456-3",
            Matricula = "MP001",
            FechaNacimiento = new DateTime(1980, 1, 1),
            Telefono = 3814123456
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
    [InlineData("invalido")]
    [InlineData("@test.com")]
    public void Validate_ConEmailInvalido_NoEsValido(string emailInvalido)
    {
        // Arrange
        var request = new RegistrarUsuarioRequest
        {
            Email = emailInvalido,
            Password = "Password123!",
            TipoAutoridad = TipoAutoridad.Medico,
            Nombre = "Juan",
            Apellido = "Pérez",
            DNI = 30123456,
            CUIL = "20-30123456-3",
            Matricula = "MP001",
            FechaNacimiento = new DateTime(1980, 1, 1)
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("short")]
    public void Validate_ConPasswordInvalido_NoEsValido(string passwordInvalido)
    {
        // Arrange
        var request = new RegistrarUsuarioRequest
        {
            Email = "doctor@hospital.com",
            Password = passwordInvalido,
            TipoAutoridad = TipoAutoridad.Medico,
            Nombre = "Juan",
            Apellido = "Pérez",
            DNI = 30123456,
            CUIL = "20-30123456-3",
            Matricula = "MP001",
            FechaNacimiento = new DateTime(1980, 1, 1)
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("20-30123456-9")] // Dígito verificador incorrecto
    [InlineData("12345")]
    [InlineData("")]
    public void Validate_ConCUILInvalido_NoEsValido(string cuilInvalido)
    {
        // Arrange
        var request = new RegistrarUsuarioRequest
        {
            Email = "doctor@hospital.com",
            Password = "Password123!",
            TipoAutoridad = TipoAutoridad.Medico,
            Nombre = "Juan",
            Apellido = "Pérez",
            DNI = 30123456,
            CUIL = cuilInvalido,
            Matricula = "MP001",
            FechaNacimiento = new DateTime(1980, 1, 1)
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "CUIL");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_SinNombre_NoEsValido(string nombreInvalido)
    {
        // Arrange
        var request = new RegistrarUsuarioRequest
        {
            Email = "doctor@hospital.com",
            Password = "Password123!",
            TipoAutoridad = TipoAutoridad.Medico,
            Nombre = nombreInvalido,
            Apellido = "Pérez",
            DNI = 30123456,
            CUIL = "20-30123456-3",
            Matricula = "MP001",
            FechaNacimiento = new DateTime(1980, 1, 1)
        };

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Nombre");
    }
}