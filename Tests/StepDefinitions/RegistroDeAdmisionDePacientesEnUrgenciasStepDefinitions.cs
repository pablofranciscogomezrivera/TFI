using System;
using Reqnroll;

namespace Tests.StepDefinitions
{
    [Binding]
    public class RegistroDeAdmisionDePacientesEnUrgenciasStepDefinitions
    {
        [Given("que la enfermera {string} está logueada en el sistema")]
        public void GivenQueLaEnfermeraEstaLogueadaEnElSistema(string p0)
        {
            throw new PendingStepException();
        }

        [Given("que el paciente con DNI {string} existe en el sistema")]
        public void GivenQueElPacienteConDNIExisteEnElSistema(string p0)
        {
            throw new PendingStepException();
        }

        [When("registro un ingreso con los siguientes datos:")]
        public void WhenRegistroUnIngresoConLosSiguientesDatos(DataTable dataTable)
        {
            throw new PendingStepException();
        }

        [Then("el ingreso se registra exitosamente con estado {string}")]
        public void ThenElIngresoSeRegistraExitosamenteConEstado(string pENDIENTE)
        {
            throw new PendingStepException();
        }

        [Then("el paciente entra a la cola de atención")]
        public void ThenElPacienteEntraALaColaDeAtencion()
        {
            throw new PendingStepException();
        }

        [Then("la enfermera {string} queda registrada en el ingreso")]
        public void ThenLaEnfermeraQuedaRegistradaEnElIngreso(string p0)
        {
            throw new PendingStepException();
        }

        [Given("que el paciente con DNI {string} no existe en el sistema")]
        public void GivenQueElPacienteConDNINoExisteEnElSistema(string p0)
        {
            throw new PendingStepException();
        }

        [When("intento registrar su ingreso con los siguientes datos:")]
        public void WhenIntentoRegistrarSuIngresoConLosSiguientesDatos(DataTable dataTable)
        {
            throw new PendingStepException();
        }

        [Then("el sistema crea al paciente")]
        public void ThenElSistemaCreaAlPaciente()
        {
            throw new PendingStepException();
        }

        [Then("el ingreso queda registrado con estado {string}")]
        public void ThenElIngresoQuedaRegistradoConEstado(string pENDIENTE)
        {
            throw new PendingStepException();
        }

        [When("intento registrar el ingreso sin informar la {string}")]
        public void WhenIntentoRegistrarElIngresoSinInformarLa(string frecuenciaCardiaca)
        {
            throw new PendingStepException();
        }

        [Then("el sistema muestra un error indicando que {string} es mandatorio")]
        public void ThenElSistemaMuestraUnErrorIndicandoQueEsMandatorio(string frecuenciaCardiaca)
        {
            throw new PendingStepException();
        }

        [When("intento registrar el ingreso con frecuenciaCardiaca {int} y frecuenciaRespiratoria {int}")]
        public void WhenIntentoRegistrarElIngresoConFrecuenciaCardiacaYFrecuenciaRespiratoria(int p0, int p1)
        {
            throw new PendingStepException();
        }

        [Then("el sistema muestra un error indicando que las frecuencias no pueden ser negativas")]
        public void ThenElSistemaMuestraUnErrorIndicandoQueLasFrecuenciasNoPuedenSerNegativas()
        {
            throw new PendingStepException();
        }

        [Given("que hay un paciente {string} en espera con nivel de emergencia {string}")]
        public void GivenQueHayUnPacienteEnEsperaConNivelDeEmergencia(string b, string urgencia)
        {
            throw new PendingStepException();
        }

        [Given("que el paciente {string} con DNI {string} existe en el sistema")]
        public void GivenQueElPacienteConDNIExisteEnElSistema(string a, string p1)
        {
            throw new PendingStepException();
        }

        [When("registro un ingreso para el paciente {string} con nivel de emergencia {string}")]
        public void WhenRegistroUnIngresoParaElPacienteConNivelDeEmergencia(string a, string crítica)
        {
            throw new PendingStepException();
        }

        [Then("el paciente {string} debe ser atendido antes que el paciente {string}")]
        public void ThenElPacienteDebeSerAtendidoAntesQueElPaciente(string a, string b)
        {
            throw new PendingStepException();
        }

        [Given("que hay un paciente {string} en espera con nivel de emergencia {string} ingresado a las {float}")]
        public void GivenQueHayUnPacienteEnEsperaConNivelDeEmergenciaIngresadoALas(string b, string emergencia, Decimal p2)
        {
            throw new PendingStepException();
        }

        [When("registro un ingreso para el paciente {string} con nivel de emergencia {string} a las {float}")]
        public void WhenRegistroUnIngresoParaElPacienteConNivelDeEmergenciaALas(string a, string emergencia, Decimal p2)
        {
            throw new PendingStepException();
        }

        [Then("el paciente {string} debe ser atendido antes que el paciente {string}")]
        public void ThenElPacienteDebeSerAtendidoAntesQueElPaciente(string b, string a)
        {
            throw new PendingStepException();
        }
    }
}
