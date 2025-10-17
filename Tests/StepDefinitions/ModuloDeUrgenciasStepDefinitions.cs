using System;
using Aplicacion;
using Dominio.Entidades;
using Dominio.Interfaces;
using Infraestructura;
using Reqnroll;
using Reqnroll.Assist;
using FluentAssertions;

namespace Tests.StepDefinitions
{
    [Binding]
    public class ModuloDeUrgenciasStepDefinitions
    {
        private Enfermera _enfermera;
        private readonly IRepositorioPacientes _dbMockeada = new DBPruebaMemoria();
        private IServicioUrgencias _servicioUrgencias;
        private Exception _excepcionEncontrada;

        public ModuloDeUrgenciasStepDefinitions()
        {
            _servicioUrgencias = new ServicioUrgencias(_dbMockeada);
        }

        [Given("que la siguiente enfermera esta registrada:")]
        public void GivenQueLaSiguienteEnfermeraEstaRegistrada(DataTable dataTable)
        {
            _enfermera = dataTable.CreateInstance<Enfermera>();
        }

        [Given("que estan registrados los siguientes pacientes:")]
        public void GivenQueEstanRegistradosLosSiguientesPacientes(DataTable dataTable)
        {
            var pacientes = dataTable.CreateSet<Paciente>();
            foreach (var paciente in pacientes)
            {
                _dbMockeada.GuardarPaciente(paciente);
            }
        }

        [When("Ingresan a urgencias los siguientes pacientes:")]
        public void WhenIngresanAUrgenciasLosSiguientesPacientes(DataTable dataTable)
        {
            _excepcionEncontrada = null;

            foreach (var fila in dataTable.Rows)
            {
                var cuil = fila["CUIL"].Trim();
                var informe = fila["Informe"].Trim();

                if (!Enum.TryParse<NivelEmergencia>(fila["Nivel de Emergencia"].Trim(), true, out var nivelEmergencia))
                {
                    throw new ArgumentException($"El nivel de Emergencia '{fila["Nivel de Emergencia"]}' no es válido.");
                }

                var temperatura = double.Parse(fila["Temperatura"]);
                var frecCardiaca = double.Parse(fila["Frecuencia Cardiaca"]);
                var frecRespiratoria = double.Parse(fila["Frecuencia Respiratoria"]);
                var tension = fila["Tension Arterial"].Split('/');
                var sistolica = double.Parse(tension[0]);
                var diastolica = double.Parse(tension[1]);

                try
                {
                    _servicioUrgencias.RegistrarUrgencia(
                        cuil,
                        _enfermera,
                        informe,
                        temperatura,
                        nivelEmergencia,
                        frecCardiaca,
                        frecRespiratoria,
                        sistolica,
                        diastolica);
                }
                catch (Exception e)
                {
                    _excepcionEncontrada = e;
                    break;
                }
            }
        }

        [Then("La lista de espera esta ordenada por cuil de la siguiente manera:")]
        public void ThenLaListaDeEsperaEstaOrdenadaPorCuilDeLaSiguienteManera(DataTable dataTable)
        {
            var cuilsEsperados = dataTable.Rows.Select(fila => fila["CUIL"]).ToList();

            var cuilsReales = _servicioUrgencias.ObtenerIngresosPendientes()
                .Select(ingreso => ingreso.Paciente.CUIL)
                .ToList();

            cuilsReales.Should().Equal(cuilsEsperados);

        }

        [Then("el sistema muestra el siguiente error: {string}")]
        public void ThenElSistemaMuestraElSiguienteError(string mensajeError)
        {
            _excepcionEncontrada.Should().NotBeNull();
            _excepcionEncontrada.Message.Should().Be(mensajeError);
        }
    }
}
