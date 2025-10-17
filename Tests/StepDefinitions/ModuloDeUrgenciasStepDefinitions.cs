using Reqnroll;
using Dominio.Entidades;
using Dominio.Interfaces;
using Infraestructura;
using Aplicacion;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.StepDefinitions
{
    [Binding]
    public class ModuloDeUrgenciasStepDefinitions
    {
        private Enfermera _enfermera;
        private IRepositorioPacientes _dbMockeada = new DBPruebaMemoria();
        private IServicioUrgencias _servicioUrgencias;
        private Exception _excepcionEsperada;

        [BeforeScenario]
        public void Setup()
        {
            _dbMockeada = new DBPruebaMemoria();
            _servicioUrgencias = new ServicioUrgencias(_dbMockeada);
            _excepcionEsperada = null;
        }

        [Given("que la siguiente enfermera esta registrada:")]
        public void GivenQueLaSiguienteEnfermeraEstaRegistrada(Table table)
        {
            var row = table.Rows[0];
            _enfermera = new Enfermera
            {
                Nombre = row["Nombre"],
                Apellido = row["Apellido"],
                Matricula = "0000",
                Usuario = new Usuario { User = "test", Password = "123" }
            };
        }

        [Given("que estan registrados los siguientes pacientes:")]
        public void GivenQueEstanRegistradosLosSiguientesPacientes(Table table)
        {
            foreach (var row in table.Rows)
            {
                var paciente = new Paciente
                {
                    CUIL = row["CUIL"].Trim(),
                    Nombre = row["Nombre"].Trim(),
                    Apellido = row["Apellido"].Trim(),
                    DNI = 0,
                    FechaNacimiento = DateTime.Now,
                    Email = "test@gmail.com",
                    Telefono = 12345,
                    Afiliado = new Afiliado
                    {
                        NumeroAfiliado = "N/A",
                        ObraSocial = new ObraSocial { Nombre = row["Obra Social"].Trim() }
                    },
                    Domicilio = new Domicilio
                    {
                        Calle = "N/A",
                        Numero = 0,
                        Ciudad = "N/A",
                        Provincia = "N/A",
                        Localidad = "N/A"
                    }
                };
                _dbMockeada.GuardarPaciente(paciente);
            }
        }

        [When("Ingresan a urgencias los siguientes pacientes:")]
        public void WhenIngresanAUrgenciasLosSiguientesPacientes(Table table)
        {
            _excepcionEsperada = null;
            foreach (var fila in table.Rows)
            {
                try
                {
                    var cuil = fila["CUIL"].Trim();
                    var informe = fila["Informe"].Trim();

                    if (string.IsNullOrEmpty(informe)) throw new ArgumentException("El informe es un dato mandatorio");
                    if (!Enum.TryParse<NivelEmergencia>(fila["Nivel de Emergencia"].Trim(), true, out var nivelEmergencia)) throw new ArgumentException($"El nivel de emergencia '{fila["Nivel de Emergencia"]}' no es válido.");
                    
                    var temperatura = double.Parse(fila["Temperatura"]);
                    var frecuenciaCardiaca = double.Parse(fila["Frecuencia Cardiaca"]);
                    var frecuenciaRespiratoria = double.Parse(fila["Frecuencia Respiratoria"]);
                    var tension = fila["Tension Arterial"].Split('/');
                    var sistolica = double.Parse(tension[0]);
                    var diastolica = double.Parse(tension[1]);

                    _servicioUrgencias.RegistrarUrgencia(cuil, _enfermera, informe, temperatura, nivelEmergencia, frecuenciaCardiaca, frecuenciaRespiratoria, sistolica, diastolica);
                }
                catch (Exception e)
                {
                    _excepcionEsperada = e.GetBaseException();
                    break;
                }
            }
        }

        [Then("el paciente con CUIL \"(.*)\" se crea en el sistema")]
        public void ThenElPacienteConCUILSeCreaEnElSistema(string cuil)
        {
            var paciente = _dbMockeada.BuscarPacientePorCuil(cuil);
            paciente.Should().NotBeNull();
            paciente.CUIL.Should().Be(cuil);
        }

        [Then("La lista de espera esta ordenada por cuil de la siguiente manera:")]
        public void ThenLaListaDeEsperaEstaOrdenadaPorCuilDeLaSiguienteManera(Table table)
        {
            var cuilsEsperados = table.Rows.Select(row => row["CUIL"].Trim()).ToList();

            var cuilsReales = _servicioUrgencias.ObtenerIngresosPendientes()
                .Select(ingreso => ingreso.Paciente.CUIL)
                .ToList();

            cuilsReales.Should().Equal(cuilsEsperados);
        }

        [Then("el sistema muestra el siguiente error: \"(.*)\"")]
        public void ThenElSistemaMuestraElSiguienteError(string mensajeError)
        {
            _excepcionEsperada.Should().NotBeNull();
            _excepcionEsperada.Message.Should().Be(mensajeError);
        }
    }
}