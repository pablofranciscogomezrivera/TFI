using Aplicacion;
using Aplicacion.Servicios;
using Dominio.Entidades;
using Dominio.Enums;
using Dominio.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Tests.UnitTests.Aplicacion.Servicios;

public class ServicioAutenticacionTests
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IRepositorioPersonal _repositorioPersonal;
    private readonly ServicioAutenticacion _servicioAutenticacion;

    public ServicioAutenticacionTests()
    {
        _repositorioUsuario = Substitute.For<IRepositorioUsuario>();
        _repositorioPersonal = Substitute.For<IRepositorioPersonal>();
        _servicioAutenticacion = new ServicioAutenticacion(_repositorioUsuario, _repositorioPersonal);
    }

    #region Tests de Registro

    [Fact]
    public void RegistrarUsuario_ConDatosValidos_CreaUsuarioConPasswordHasheado()
    {
        // Arrange
        string email = "doctor@hospital.com";
        string password = "Password123!";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        _repositorioUsuario.ExisteUsuarioConEmail(email).Returns(false);

        _repositorioUsuario.GuardarUsuario(Arg.Any<Usuario>())
            .Returns(callInfo => {
                var u = callInfo.Arg<Usuario>();
                u.Id = 1;
                return u;
            });

        // Act
        var resultado = _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Email.Should().Be(email);
        resultado.TipoAutoridad.Should().Be(tipo);

        resultado.PasswordHash.Should().NotBeNullOrEmpty();
        resultado.PasswordHash.Should().NotBe(password);
        resultado.PasswordHash.Should().StartWith("$2");

        _repositorioUsuario.Received(1).GuardarUsuario(Arg.Is<Usuario>(u =>
            u.Email == email &&
            u.TipoAutoridad == tipo &&
            u.PasswordHash != password
        ));
    }

    [Fact]
    public void RegistrarUsuarioConEmpleado_ConDoctorValido_CreaUsuarioYDoctor()
    {
        // Arrange
        string email = "doctor@hospital.com";
        string password = "Password123!";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        _repositorioUsuario.ExisteUsuarioConEmail(email).Returns(false);

        Usuario usuarioGuardado = null;
        _repositorioUsuario.GuardarUsuario(Arg.Do<Usuario>(x => usuarioGuardado = x))
            .Returns(callInfo => {
                var u = callInfo.Arg<Usuario>();
                u.Id = 1;
                return u;
            });

        // Act
        var resultado = _servicioAutenticacion.RegistrarUsuarioConEmpleado(
            email, password, tipo, "Juan", "Pérez", 30123456, "20-30123456-3",
            "MP001", new DateTime(1980, 1, 1), 3814123456
        );

        // Assert
        resultado.Should().NotBeNull();
        usuarioGuardado.Should().NotBeNull();

        usuarioGuardado.PasswordHash.Should().NotBe(password);
        usuarioGuardado.PasswordHash.Should().StartWith("$2");

        _repositorioPersonal.Received(1).GuardarDoctor(Arg.Is<Doctor>(d =>
            d.Nombre == "Juan" &&
            d.Apellido == "Pérez" &&
            d.Matricula == "MP001"
        ), 1);
    }

    [Fact]
    public void RegistrarUsuarioConEmpleado_ConEnfermeraValida_CreaUsuarioYEnfermera()
    {
        // Arrange
        string email = "enfermera@hospital.com";
        string password = "Password123!";
        TipoAutoridad tipo = TipoAutoridad.Enfermera;

        _repositorioUsuario.ExisteUsuarioConEmail(email).Returns(false);

        _repositorioUsuario.GuardarUsuario(Arg.Any<Usuario>())
            .Returns(callInfo => {
                var u = callInfo.Arg<Usuario>();
                u.Id = 2;
                return u;
            });

        // Act
        var resultado = _servicioAutenticacion.RegistrarUsuarioConEmpleado(
            email, password, tipo, "María", "González", 25123456, "27-25123456-1",
            "ENF001", new DateTime(1985, 5, 15), 3814654321
        );

        // Assert
        resultado.Should().NotBeNull();
        _repositorioUsuario.Received(1).GuardarUsuario(Arg.Any<Usuario>());

        _repositorioPersonal.Received(1).GuardarEnfermera(Arg.Is<Enfermera>(e =>
            e.Nombre == "María" &&
            e.Matricula == "ENF001"
        ), 2);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RegistrarUsuario_ConEmailInvalido_LanzaExcepcion(string emailInvalido)
    {
        Action act = () => _servicioAutenticacion.RegistrarUsuario(
            emailInvalido, "password123", TipoAutoridad.Medico
        );

        act.Should().Throw<ArgumentException>().WithMessage("*email*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("short")]
    public void RegistrarUsuario_ConPasswordInvalido_LanzaExcepcion(string passwordInvalido)
    {
        Action act = () => _servicioAutenticacion.RegistrarUsuario(
            "email@test.com", passwordInvalido, TipoAutoridad.Medico
        );

        act.Should().Throw<ArgumentException>().WithMessage("*contraseña*");
    }

    #endregion

    #region Tests de Login

    [Fact]
    public void Login_ConCredencialesValidas_RetornaUsuario()
    {
        // Arrange
        string email = "doctor@hospital.com";
        string password = "Password123!";

        string passwordHashReal = BCrypt.Net.BCrypt.HashPassword(password);

        var usuarioEnDb = new Usuario
        {
            Id = 1,
            Email = email,
            PasswordHash = passwordHashReal,
            TipoAutoridad = TipoAutoridad.Medico
        };

        _repositorioUsuario.BuscarPorEmail(email).Returns(usuarioEnDb);

        // Act
        var resultado = _servicioAutenticacion.IniciarSesion(email, password);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Email.Should().Be(email);
        _repositorioUsuario.Received(1).BuscarPorEmail(email);
    }

    [Fact]
    public void Login_ConPasswordIncorrecto_LanzaExcepcion()
    {
        // Arrange
        string email = "doctor@hospital.com";
        string passwordCorrecta = "Password123!";
        string passwordIncorrecta = "WrongPassword";

        string passwordHashReal = BCrypt.Net.BCrypt.HashPassword(passwordCorrecta);

        var usuarioEnDb = new Usuario
        {
            Id = 1,
            Email = email,
            PasswordHash = passwordHashReal,
            TipoAutoridad = TipoAutoridad.Medico
        };

        _repositorioUsuario.BuscarPorEmail(email).Returns(usuarioEnDb);

        // Act
        Action act = () => _servicioAutenticacion.IniciarSesion(email, passwordIncorrecta);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Usuario o contraseña inválidos*");
    }

    [Fact]
    public void Login_ConEmailInexistente_LanzaExcepcion()
    {
        // Arrange
        string email = "noexiste@hospital.com";
        _repositorioUsuario.BuscarPorEmail(email).Returns((Usuario?)null);

        // Act
        Action act = () => _servicioAutenticacion.IniciarSesion(email, "password");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Usuario o contraseña inválidos*");
    }

    #endregion
}