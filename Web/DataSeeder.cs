using Dominio.Entidades;
using Dominio.Interfaces;

namespace Web;

/// <summary>
/// Clase para poblar la base de datos con datos de prueba al iniciar la aplicación
/// </summary>
public static class DataSeeder
{
    public static void SeedData(IRepositorioPacientes repositorioPacientes, IRepositorioObraSocial repositorioObraSocial)
    {
        // Sembrar Obras Sociales
        SeedObrasSociales(repositorioObraSocial);

        // Sembrar Pacientes de prueba
        SeedPacientes(repositorioPacientes, repositorioObraSocial);
    }

    private static void SeedObrasSociales(IRepositorioObraSocial repositorio)
    {
        var obrasSociales = new List<ObraSocial>
        {
            new ObraSocial { Id = 1, Nombre = "OSDE" },
            new ObraSocial { Id = 2, Nombre = "Swiss Medical" },
            new ObraSocial { Id = 3, Nombre = "Subsidio de Salud" },
            new ObraSocial { Id = 4, Nombre = "Fondo de Bikini SA" },
            new ObraSocial { Id = 5, Nombre = "Galeno" },
        };

        foreach (var obraSocial in obrasSociales)
        {
            try
            {
                repositorio.AgregarObraSocial(obraSocial);
            }
            catch
            {
                // Ignorar si ya existe
            }
        }
    }

    private static void SeedPacientes(IRepositorioPacientes repositorioPacientes, IRepositorioObraSocial repositorioObraSocial)
    {
        var osde = repositorioObraSocial.BuscarObraSocialPorId(1);
        var swissMedical = repositorioObraSocial.BuscarObraSocialPorId(2);
        var subsidio = repositorioObraSocial.BuscarObraSocialPorId(3);
        var bikini = repositorioObraSocial.BuscarObraSocialPorId(4);

        var pacientes = new List<Paciente>
        {
            new Paciente
            {
                CUIL = "20-12345678-9",
                DNI = 12345678,
                Nombre = "Juan",
                Apellido = "Pérez",
                FechaNacimiento = new DateTime(1985, 5, 15),
                Email = "juan.perez@email.com",
                Telefono = 3814567890,
                Domicilio = new Domicilio
                {
                    Calle = "San Martín",
                    Numero = 123,
                    Localidad = "San Miguel de Tucumán",
                    Ciudad = "San Miguel de Tucumán",
                    Provincia = "Tucumán"
                },
                Afiliado = new Afiliado
                {
                    NumeroAfiliado = "12345",
                    ObraSocial = osde
                }
            },
            new Paciente
            {
                CUIL = "27-23456789-0",
                DNI = 23456789,
                Nombre = "María",
                Apellido = "García",
                FechaNacimiento = new DateTime(1990, 8, 22),
                Email = "maria.garcia@email.com",
                Telefono = 3814567891,
                Domicilio = new Domicilio
                {
                    Calle = "Belgrano",
                    Numero = 456,
                    Localidad = "Yerba Buena",
                    Ciudad = "Yerba Buena",
                    Provincia = "Tucumán"
                },
                Afiliado = new Afiliado
                {
                    NumeroAfiliado = "67890",
                    ObraSocial = swissMedical
                }
            },
            new Paciente
            {
                CUIL = "23-34567890-1",
                DNI = 34567890,
                Nombre = "Carlos",
                Apellido = "López",
                FechaNacimiento = new DateTime(1975, 3, 10),
                Email = "carlos.lopez@email.com",
                Telefono = 3814567892,
                Domicilio = new Domicilio
                {
                    Calle = "Avenida Alem",
                    Numero = 789,
                    Localidad = "Tafí Viejo",
                    Ciudad = "Tafí Viejo",
                    Provincia = "Tucumán"
                },
                Afiliado = new Afiliado
                {
                    NumeroAfiliado = "ABC123",
                    ObraSocial = subsidio
                }
            },
            new Paciente
            {
                CUIL = "20-45678901-2",
                DNI = 45678901,
                Nombre = "Ana",
                Apellido = "Martínez",
                FechaNacimiento = new DateTime(1988, 11, 5),
                Email = "ana.martinez@email.com",
                Telefono = 3814567893,
                Domicilio = new Domicilio
                {
                    Calle = "Mate de Luna",
                    Numero = 321,
                    Localidad = "San Miguel de Tucumán",
                    Ciudad = "San Miguel de Tucumán",
                    Provincia = "Tucumán"
                },
                Afiliado = new Afiliado
                {
                    NumeroAfiliado = "XYZ789",
                    ObraSocial = bikini
                }
            },
            new Paciente
            {
                CUIL = "23-1234567-9",
                DNI = 1234567,
                Nombre = "Marcelo",
                Apellido = "Nunez",
                FechaNacimiento = new DateTime(1980, 7, 20),
                Email = "marcelo.nunez@email.com",
                Telefono = 3814567894,
                Domicilio = new Domicilio
                {
                    Calle = "Las Piedras",
                    Numero = 555,
                    Localidad = "San Miguel de Tucumán",
                    Ciudad = "San Miguel de Tucumán",
                    Provincia = "Tucumán"
                },
                Afiliado = new Afiliado
                {
                    NumeroAfiliado = "MN001",
                    ObraSocial = subsidio
                }
            },
            new Paciente
            {
                CUIL = "23-4567899-2",
                DNI = 4567899,
                Nombre = "Patricio",
                Apellido = "Estrella",
                FechaNacimiento = new DateTime(1995, 2, 14),
                Email = "patricio.estrella@email.com",
                Telefono = 3814567895,
                Domicilio = new Domicilio
                {
                    Calle = "Fondo de Bikini",
                    Numero = 1,
                    Localidad = "San Miguel de Tucumán",
                    Ciudad = "San Miguel de Tucumán",
                    Provincia = "Tucumán"
                },
                Afiliado = new Afiliado
                {
                    NumeroAfiliado = "PE002",
                    ObraSocial = bikini
                }
            }
        };

        foreach (var paciente in pacientes)
        {
            try
            {
                var existente = repositorioPacientes.BuscarPacientePorCuil(paciente.CUIL);
                if (existente == null)
                {
                    repositorioPacientes.GuardarPaciente(paciente);
                }
            }
            catch
            {
                // Ignorar errores de duplicados
            }
        }
    }
}