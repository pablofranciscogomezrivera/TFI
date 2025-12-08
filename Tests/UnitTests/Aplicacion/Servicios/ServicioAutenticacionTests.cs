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

    [Fact]
    public void RegistrarUsuario_ConDatosValidos_CreaUsuarioConPasswordHasheado()
    {
        // Arrange
        string email = "doctor@hospital.com";
        string password = "Password123!";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        _repositorioUsuario.GuardarUsuario(Arg.Any<Usuario>())
            .Returns(callInfo => callInfo.Arg<Usuario>());

        // Act
        var resultado = _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Email.Should().Be(email);
        resultado.TipoAutoridad.Should().Be(tipo);
        resultado.PasswordHash.Should().NotBe(password); // Debe estar hasheado
        resultado.PasswordHash.Should().NotBeNullOrEmpty();
        _repositorioUsuario.Received(1).GuardarUsuario(Arg.Is<Usuario>(u =>
            u.Email == email &&
            u.TipoAutoridad == tipo &&
            u.PasswordHash != password
        ));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RegistrarUsuario_ConEmailInvalido_LanzaExcepcion(string emailInvalido)
    {
        // Act
        Action act = () => _servicioAutenticacion.RegistrarUsuario(
            emailInvalido, "password", TipoAutoridad.Medico
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*email*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RegistrarUsuario_ConPasswordInvalido_LanzaExcepcion(string passwordInvalido)
    {
        // Act
        Action act = () => _servicioAutenticacion.RegistrarUsuario(
            "email@test.com", passwordInvalido, TipoAutoridad.Medico
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*contraseña*");
    }

    [Fact]
    public void RegistrarUsuarioConEmpleado_ConDoctorValido_CreaUsuarioYDoctor()
    {
        // Arrange
        string email = "doctor@hospital.com";
        string password = "Password123!";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        var usuarioGuardado = new Usuario
        {
            Id = 1,
            Email = email,
            PasswordHash = "hashedpassword",
            TipoAutoridad = tipo
        };

        _repositorioUsuario.GuardarUsuario(Arg.Any<Usuario>()).Returns(usuarioGuardado);
        _repositorioPersonal.GuardarDoctor(Arg.Any<Doctor>(), usuarioGuardado.Id);
            

        // Act
        var resultado = _servicioAutenticacion.RegistrarUsuarioConEmpleado(
            email, password, tipo, "Juan", "Pérez", 30123456, "20-30123456-3",
            "MP001", new DateTime(1980, 1, 1), 3814123456
        );

        // Assert
        resultado.Should().NotBeNull();
        resultado.Email.Should().Be(email);
        _repositorioUsuario.Received(1).GuardarUsuario(Arg.Any<Usuario>());
        _repositorioPersonal.Received(1).GuardarDoctor(Arg.Is<Doctor>(d =>
            d.Usuario.Id == usuarioGuardado.Id &&
            d.Nombre == "Juan" &&
            d.Apellido == "Pérez" &&
            d.Matricula == "MP001"
        ), usuarioGuardado.Id);
        _repositorioPersonal.DidNotReceive().GuardarEnfermera(Arg.Any<Enfermera>(), usuarioGuardado.Id);
    }

    [Fact]
    public void RegistrarUsuarioConEmpleado_ConEnfermeraValida_CreaUsuarioYEnfermera()
    {
        // Arrange
        string email = "enfermera@hospital.com";
        string password = "Password123!";
        TipoAutoridad tipo = TipoAutoridad.Enfermera;

        var usuarioGuardado = new Usuario
        {
            Id = 2,
            Email = email,
            PasswordHash = "hashedpassword",
            TipoAutoridad = tipo
        };

        _repositorioUsuario.GuardarUsuario(Arg.Any<Usuario>()).Returns(usuarioGuardado);
        _repositorioPersonal.GuardarEnfermera(Arg.Any<Enfermera>(), usuarioGuardado.Id);

        // Act
        var resultado = _servicioAutenticacion.RegistrarUsuarioConEmpleado(
            email, password, tipo, "María", "González", 25123456, "27-25123456-0",
            "ENF001", new DateTime(1985, 5, 15), 3814654321
        );

        // Assert
        resultado.Should().NotBeNull();
        _repositorioUsuario.Received(1).GuardarUsuario(Arg.Any<Usuario>());
        _repositorioPersonal.Received(1).GuardarEnfermera(Arg.Is<Enfermera>(e =>
            e.Usuario.Id == usuarioGuardado.Id &&
            e.Nombre == "María" &&
            e.Apellido == "González" &&
            e.Matricula == "ENF001"
        ), usuarioGuardado.Id);
        _repositorioPersonal.DidNotReceive().GuardarDoctor(Arg.Any<Doctor>(), usuarioGuardado.Id);
    }

    [Fact]
    public void Login_ConCredencialesValidas_RetornaUsuario()
    {
        // Arrange
        string email = "doctor@hospital.com";
        string password = "Password123!";
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var usuario = new Usuario
        {
            Id = 1,
            Email = email,
            PasswordHash = passwordHash,
            TipoAutoridad = TipoAutoridad.Medico
        };

        _repositorioUsuario.BuscarPorEmail(email).Returns(usuario);

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
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(passwordCorrecta);

        var usuario = new Usuario
        {
            Id = 1,
            Email = email,
            PasswordHash = passwordHash,
            TipoAutoridad = TipoAutoridad.Medico
        };

        _repositorioUsuario.BuscarPorEmail(email).Returns(usuario);

        // Act
        Action act = () => _servicioAutenticacion.IniciarSesion(email, passwordIncorrecta);

        // Assert
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("*Credenciales inválidas*");
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
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("*Credenciales inválidas*");
    }
}