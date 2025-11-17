using Aplicacion;
using Dominio.Entidades;
using Dominio.Enums;
using Dominio.Interfaces;
using FluentAssertions;
using Infraestructura;
using Xunit;

namespace Tests.UnitTests;

public class ServicioAutenticacionTests
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly ServicioAutenticacion _servicioAutenticacion;

    public ServicioAutenticacionTests()
    {
        _repositorioUsuario = new RepositorioUsuarioMemoria();
        _servicioAutenticacion = new ServicioAutenticacion(_repositorioUsuario);
    }

    #region Tests de Registro

    [Fact]
    public void RegistrarUsuario_ConDatosValidosMedico_RegistroExitoso()
    {
        // Arrange
        string email = "doctor@hospital.com";
        string password = "Password123!";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        // Act
        var usuario = _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        usuario.Should().NotBeNull();
        usuario.Id.Should().BeGreaterThan(0);
        usuario.Email.Should().Be(email);
        usuario.TipoAutoridad.Should().Be(TipoAutoridad.Medico);
        usuario.PasswordHash.Should().NotBeNullOrEmpty();
        usuario.PasswordHash.Should().NotBe(password); // Verificar que la contraseña está hasheada
    }

    [Fact]
    public void RegistrarUsuario_ConDatosValidosEnfermera_RegistroExitoso()
    {
        // Arrange
        string email = "enfermera@hospital.com";
        string password = "SecurePass2024";
        TipoAutoridad tipo = TipoAutoridad.Enfermera;

        // Act
        var usuario = _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        usuario.Should().NotBeNull();
        usuario.Email.Should().Be(email);
        usuario.TipoAutoridad.Should().Be(TipoAutoridad.Enfermera);
        usuario.PasswordHash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void RegistrarUsuario_EmailVacio_LanzaExcepcion()
    {
        // Arrange
        string email = "";
        string password = "Password123";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        // Act
        Action act = () => _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El email es un campo mandatorio");
    }

    [Fact]
    public void RegistrarUsuario_EmailNull_LanzaExcepcion()
    {
        // Arrange
        string email = null;
        string password = "Password123";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        // Act
        Action act = () => _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El email es un campo mandatorio");
    }

    [Theory]
    [InlineData("emailinvalido")]
    [InlineData("@hospital.com")]
    [InlineData("usuario@")]
    [InlineData("usuario @hospital.com")]
    public void RegistrarUsuario_EmailFormatoInvalido_LanzaExcepcion(string email)
    {
        // Arrange
        string password = "Password123";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        // Act
        Action act = () => _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El email no tiene un formato válido");
    }

    [Fact]
    public void RegistrarUsuario_ContraseñaVacia_LanzaExcepcion()
    {
        // Arrange
        string email = "doctor@hospital.com";
        string password = "";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        // Act
        Action act = () => _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("La contraseña es un campo mandatorio");
    }

    [Fact]
    public void RegistrarUsuario_ContraseñaNull_LanzaExcepcion()
    {
        // Arrange
        string email = "doctor@hospital.com";
        string password = null;
        TipoAutoridad tipo = TipoAutoridad.Medico;

        // Act
        Action act = () => _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("La contraseña es un campo mandatorio");
    }

    [Theory]
    [InlineData("1234567")]  // 7 caracteres
    [InlineData("Pass123")]  // 7 caracteres
    [InlineData("short")]    // 5 caracteres
    [InlineData("a")]        // 1 caracter
    public void RegistrarUsuario_ContraseñaMenosDe8Caracteres_LanzaExcepcion(string password)
    {
        // Arrange
        string email = "doctor@hospital.com";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        // Act
        Action act = () => _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("La contraseña debe tener al menos 8 caracteres");
    }

    [Fact]
    public void RegistrarUsuario_EmailYaRegistrado_LanzaExcepcion()
    {
        // Arrange
        string email = "doctor@hospital.com";
        string password1 = "Password123";
        string password2 = "OtraPassword456";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        // Registrar el primer usuario
        _servicioAutenticacion.RegistrarUsuario(email, password1, tipo);

        // Act - Intentar registrar otro usuario con el mismo email
        Action act = () => _servicioAutenticacion.RegistrarUsuario(email, password2, tipo);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Ya existe un usuario registrado con ese email");
    }

    [Fact]
    public void RegistrarUsuario_ContraseñaEsHasheada_NoSeGuardaEnTextoPlano()
    {
        // Arrange
        string email = "seguridad@hospital.com";
        string password = "MiPasswordSegura123";
        TipoAutoridad tipo = TipoAutoridad.Enfermera;

        // Act
        var usuario = _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        usuario.PasswordHash.Should().NotBe(password);
        usuario.PasswordHash.Should().StartWith("$2"); // BCrypt hash starts with $2
        usuario.PasswordHash.Length.Should().Be(60); // BCrypt hash length is 60
    }

    #endregion

    #region Tests de Inicio de Sesión

    [Fact]
    public void IniciarSesion_ConCredencialesValidas_RetornaUsuario()
    {
        // Arrange
        string email = "login@hospital.com";
        string password = "Password123";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        // Registrar usuario primero
        _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Act
        var usuario = _servicioAutenticacion.IniciarSesion(email, password);

        // Assert
        usuario.Should().NotBeNull();
        usuario.Email.Should().Be(email);
        usuario.TipoAutoridad.Should().Be(tipo);
    }

    [Fact]
    public void IniciarSesion_ConContraseñaIncorrecta_LanzaExcepcion()
    {
        // Arrange
        string email = "test@hospital.com";
        string passwordCorrecta = "Password123";
        string passwordIncorrecta = "WrongPassword";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        // Registrar usuario
        _servicioAutenticacion.RegistrarUsuario(email, passwordCorrecta, tipo);

        // Act
        Action act = () => _servicioAutenticacion.IniciarSesion(email, passwordIncorrecta);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Usuario o contraseña inválidos");
    }

    [Fact]
    public void IniciarSesion_ConUsuarioNoExistente_LanzaExcepcion()
    {
        // Arrange
        string email = "noexiste@hospital.com";
        string password = "Password123";

        // Act
        Action act = () => _servicioAutenticacion.IniciarSesion(email, password);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Usuario o contraseña inválidos");
    }

    [Fact]
    public void IniciarSesion_EmailVacio_LanzaExcepcion()
    {
        // Arrange
        string email = "";
        string password = "Password123";

        // Act
        Action act = () => _servicioAutenticacion.IniciarSesion(email, password);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Usuario o contraseña inválidos");
    }

    [Fact]
    public void IniciarSesion_ContraseñaVacia_LanzaExcepcion()
    {
        // Arrange
        string email = "test@hospital.com";
        string password = "";

        // Act
        Action act = () => _servicioAutenticacion.IniciarSesion(email, password);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Usuario o contraseña inválidos");
    }

    [Fact]
    public void IniciarSesion_EmailNull_LanzaExcepcion()
    {
        // Arrange
        string email = null;
        string password = "Password123";

        // Act
        Action act = () => _servicioAutenticacion.IniciarSesion(email, password);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Usuario o contraseña inválidos");
    }

    [Fact]
    public void IniciarSesion_ContraseñaNull_LanzaExcepcion()
    {
        // Arrange
        string email = "test@hospital.com";
        string password = null;

        // Act
        Action act = () => _servicioAutenticacion.IniciarSesion(email, password);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Usuario o contraseña inválidos");
    }

    [Fact]
    public void IniciarSesion_MensajeErrorGenericoNoRevela_SiUsuarioExiste()
    {
        // Arrange
        string emailRegistrado = "existente@hospital.com";
        string emailNoRegistrado = "noexistente@hospital.com";
        string passwordCorrecta = "Password123";
        string passwordIncorrecta = "WrongPassword";

        // Registrar un usuario
        _servicioAutenticacion.RegistrarUsuario(emailRegistrado, passwordCorrecta, TipoAutoridad.Medico);

        // Act & Assert - Usuario no existe
        Action actUsuarioNoExiste = () => _servicioAutenticacion.IniciarSesion(emailNoRegistrado, passwordCorrecta);

        // Act & Assert - Usuario existe pero contraseña incorrecta
        Action actContraseñaIncorrecta = () => _servicioAutenticacion.IniciarSesion(emailRegistrado, passwordIncorrecta);

        // Ambos deben lanzar el mismo mensaje de error genérico
        actUsuarioNoExiste.Should().Throw<ArgumentException>()
            .WithMessage("Usuario o contraseña inválidos");

        actContraseñaIncorrecta.Should().Throw<ArgumentException>()
            .WithMessage("Usuario o contraseña inválidos");
    }

    [Fact]
    public void IniciarSesion_EmailCaseInsensitive_FuncionaCorrectamente()
    {
        // Arrange
        string emailRegistrado = "Doctor@Hospital.COM";
        string emailLogin = "doctor@hospital.com";
        string password = "Password123";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        // Registrar usuario con email en mayúsculas/minúsculas mezcladas
        _servicioAutenticacion.RegistrarUsuario(emailRegistrado, password, tipo);

        // Act - Intentar iniciar sesión con email en minúsculas
        var usuario = _servicioAutenticacion.IniciarSesion(emailLogin, password);

        // Assert
        usuario.Should().NotBeNull();
        usuario.Email.Should().Be(emailRegistrado);
    }

    #endregion

    #region Tests de Roles y Autoridades

    [Fact]
    public void RegistrarUsuario_MedicoTieneRolCorrecto()
    {
        // Arrange
        string email = "medico@hospital.com";
        string password = "Password123";
        TipoAutoridad tipo = TipoAutoridad.Medico;

        // Act
        var usuario = _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        usuario.TipoAutoridad.Should().Be(TipoAutoridad.Medico);
    }

    [Fact]
    public void RegistrarUsuario_EnfermeraTieneRolCorrecto()
    {
        // Arrange
        string email = "enfermera@hospital.com";
        string password = "Password123";
        TipoAutoridad tipo = TipoAutoridad.Enfermera;

        // Act
        var usuario = _servicioAutenticacion.RegistrarUsuario(email, password, tipo);

        // Assert
        usuario.TipoAutoridad.Should().Be(TipoAutoridad.Enfermera);
    }

    #endregion
}
