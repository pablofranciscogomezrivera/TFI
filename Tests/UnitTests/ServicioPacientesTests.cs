using Aplicacion;
using Dominio.Entidades;
using Dominio.Interfaces;
using FluentAssertions;
using Infraestructura.Memory;
using Xunit;

namespace Tests.UnitTests;

public class ServicioPacientesTests
{
    private readonly IRepositorioPacientes _repositorioPacientes;
    private readonly RepositorioObraSocialMemoria _repositorioObraSocial;
    private readonly ServicioPacientes _servicioPacientes;

    public ServicioPacientesTests()
    {
        _repositorioPacientes = new RepositorioPacientesMemoria();
        _repositorioObraSocial = new RepositorioObraSocialMemoria();
        _servicioPacientes = new ServicioPacientes(_repositorioPacientes, _repositorioObraSocial);
    }

    [Fact]
    public void RegistrarPaciente_ConTodosDatosMandatoriosYObraSocialExistente_CreaciónExitosa()
    {
        // Arrange
        var obraSocial = new ObraSocial { Id = 1, Nombre = "OSDE" };
        _repositorioObraSocial.AgregarObraSocial(obraSocial);
        _repositorioObraSocial.AgregarAfiliado(1, "12345");

        string cuil = "20-30123456-3";
        string nombre = "Juan";
        string apellido = "Pérez";
        string calle = "San Martín";
        int numero = 123;
        string localidad = "San Miguel de Tucumán";
        DateTime fechaNacimiento = new DateTime(1990, 1, 1);
        int obraSocialId = 1;
        string numeroAfiliado = "12345";


        // Act
        var paciente = _servicioPacientes.RegistrarPaciente(
            cuil, nombre, apellido, calle, numero, localidad, fechaNacimiento, obraSocialId, numeroAfiliado);

        // Assert
        paciente.Should().NotBeNull();
        paciente.CUIL.Should().Be(cuil);
        paciente.Nombre.Should().Be(nombre);
        paciente.Apellido.Should().Be(apellido);
        paciente.Domicilio.Should().NotBeNull();
        paciente.Domicilio.Calle.Should().Be(calle);
        paciente.Domicilio.Numero.Should().Be(numero);
        paciente.Domicilio.Localidad.Should().Be(localidad);
        paciente.Afiliado.Should().NotBeNull();
        paciente.Afiliado.ObraSocial.Should().NotBeNull();
        paciente.Afiliado.ObraSocial.Id.Should().Be(obraSocialId);
        paciente.Afiliado.NumeroAfiliado.Should().Be(numeroAfiliado);
    }

    [Fact]
    public void RegistrarPaciente_ConTodosDatosMandatoriosSinObraSocial_CreaciónExitosa()
    {
        // Arrange
        string cuil = "20-30123456-3";
        string nombre = "María";
        string apellido = "González";
        string calle = "Avenida Sarmiento";
        int numero = 456;
        string localidad = "Yerba Buena";
        DateTime fechaNacimiento = new DateTime(1985, 5, 15);

        // Act
        var paciente = _servicioPacientes.RegistrarPaciente(
            cuil, nombre, apellido, calle, numero, localidad, fechaNacimiento);

        // Assert
        paciente.Should().NotBeNull();
        paciente.CUIL.Should().Be(cuil);
        paciente.Nombre.Should().Be(nombre);
        paciente.Apellido.Should().Be(apellido);
        paciente.Domicilio.Should().NotBeNull();
        paciente.Domicilio.Calle.Should().Be(calle);
        paciente.Domicilio.Numero.Should().Be(numero);
        paciente.Domicilio.Localidad.Should().Be(localidad);
        paciente.Afiliado.Should().BeNull();
    }

    [Fact]
    public void RegistrarPaciente_ConObraSocialInexistente_LanzaExcepcion()
    {
        // Arrange
        string cuil = "20-30123456-3";
        string nombre = "Carlos";
        string apellido = "Rodríguez";
        string calle = "Calle Falsa";
        int numero = 789;
        string localidad = "Tafí Viejo";
        int obraSocialId = 999; // Obra social que no existe
        string numeroAfiliado = "54321";
        DateTime fechaNacimiento = new DateTime(1970, 12, 31);

        // Act
        Action act = () => _servicioPacientes.RegistrarPaciente(
            cuil, nombre, apellido, calle, numero, localidad, fechaNacimiento, obraSocialId, numeroAfiliado);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("No se puede registrar al paciente con una obra social inexistente");
    }

    [Fact]
    public void RegistrarPaciente_ConObraSocialExistenteYNoAfiliado_LanzaExcepcion()
    {
        // Arrange
        var obraSocial = new ObraSocial { Id = 2, Nombre = "Swiss Medical" };
        _repositorioObraSocial.AgregarObraSocial(obraSocial);
        _repositorioObraSocial.AgregarAfiliado(2, "11111");

        string cuil = "20-30123456-3";
        string nombre = "Laura";
        string apellido = "Fernández";
        string calle = "Las Heras";
        int numero = 321;
        string localidad = "Banda del Río Salí";
        int obraSocialId = 2;
        string numeroAfiliado = "99999"; // Número de afiliado que no existe
        DateTime fechaNacimiento = new DateTime(1995, 7, 20);

        // Act
        Action act = () => _servicioPacientes.RegistrarPaciente(
            cuil, nombre, apellido, calle, numero, localidad, fechaNacimiento, obraSocialId, numeroAfiliado);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("No se puede registrar el paciente dado que no está afiliado a la obra social");
    }

    [Theory]
    [InlineData("", "Juan", "Pérez", "San Martín", 123, "Tucumán", "1995-5-20", "El CUIL es un campo mandatorio")]
    [InlineData("20-30123456-2", "", "Pérez", "San Martín", 123, "Tucumán", "1995-5-20", "El Nombre es un campo mandatorio")]
    [InlineData("20-30123456-2", "Juan", "", "San Martín", 123, "Tucumán", "1995-5-20", "El Apellido es un campo mandatorio")]
    [InlineData("20-30123456-2", "Juan", "Pérez", "", 123, "Tucumán", "1995-5-20", "La Calle es un campo mandatorio")]
    [InlineData("20-30123456-2", "Juan", "Pérez", "San Martín", 123, "", "1995-5-20", "La Localidad es un campo mandatorio")]
    [InlineData("20-30123456-2", "Pedro", "Pérez", "San Martín", 123, "", "1995-5-20", "La Localidad es un campo mandatorio")]
    [InlineData("20-30123456-2", "Jose", "Pérez", "San Martín", 123, "", "1995-5-20", "La Localidad es un campo mandatorio")]
    public void RegistrarPaciente_ConDatoMandatorioOmitido_LanzaExcepcion(
        string cuil, string nombre, string apellido, string calle, int numero, string localidad, DateTime fechaNacimiento, string mensajeEsperado)
    {
        // Act
        Action act = () => _servicioPacientes.RegistrarPaciente(
            cuil, nombre, apellido, calle, numero, localidad, fechaNacimiento);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage(mensajeEsperado);
    }

    [Fact]
    public void RegistrarPaciente_ConNumeroMenorOIgualACero_LanzaExcepcion()
    {
        // Arrange
        string cuil = "20-30123456-3";
        string nombre = "Pedro";
        string apellido = "López";
        string calle = "Mitre";
        int numero = 0;
        string localidad = "Tucumán";
        DateTime fechaNacimiento = new DateTime(1992, 6, 15);

        // Act
        Action act = () => _servicioPacientes.RegistrarPaciente(
            cuil, nombre, apellido, calle, numero, localidad, fechaNacimiento);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El Numero debe ser mayor a 0");
    }

    [Fact]
    public void RegistrarPaciente_ConCUILInvalido_LanzaExcepcion()
    {
        // Arrange
        string cuil = "20-30123456-9"; // CUIL con dígito verificador incorrecto
        string nombre = "Ana";
        string apellido = "Martínez";
        string calle = "Belgrano";
        int numero = 555;
        string localidad = "Tucumán";
        DateTime fechaNacimiento = new DateTime(1992, 6, 25);

        // Act
        Action act = () => _servicioPacientes.RegistrarPaciente(
            cuil, nombre, apellido, calle, numero, localidad, fechaNacimiento);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El CUIL no tiene un formato válido");
    }

    [Fact]
    public void RegistrarPaciente_ConObraSocialPeroSinNumeroAfiliado_LanzaExcepcion()
    {
        // Arrange
        var obraSocial = new ObraSocial { Id = 3, Nombre = "Galeno" };
        _repositorioObraSocial.AgregarObraSocial(obraSocial);

        string cuil = "20-30123456-3";
        string nombre = "Roberto";
        string apellido = "Sánchez";
        string calle = "Rivadavia";
        int numero = 888;
        string localidad = "Tucumán";
        int obraSocialId = 3;
        string? numeroAfiliado = null;
        DateTime fechaNacimiento = new DateTime(1980, 3, 10);

        // Act
        Action act = () => _servicioPacientes.RegistrarPaciente(
            cuil, nombre, apellido, calle, numero, localidad, fechaNacimiento, obraSocialId, numeroAfiliado);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("El número de afiliado es mandatorio cuando se especifica una obra social");
    }
}