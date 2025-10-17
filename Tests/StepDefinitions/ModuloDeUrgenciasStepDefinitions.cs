using System;
using Dominio.Entidades;
using Infraestructura;
using Reqnroll;
using Xunit;
using FluentAssertions;

namespace Tests.StepDefinitions
{
    [Binding]
    public class ModuloDeUrgenciasStepDefinitions
    {
        public DBPruebaMemoria DbMockeada;

        public ModuloDeUrgenciasStepDefinitions() 
        {
            DbMockeada = new DBPruebaMemoria();
        }
        
        [Given("que la siguiente enfermera esta registrada:")]
        public void GivenQueLaSiguienteEnfermeraEstaRegistrada(DataTable dataTable)
        {
            var enfermera = dataTable.CreateInstance<Enfermera>();
        }

        [Given("Dado que estan registrados los siguientes pacientes:")]
        public void GivenQueEstanRegistradosLosSiguientesPacientes(DataTable dataTable)
        {
            var pacientes = dataTable.CreateSet<Paciente>();

            foreach (var paciente in pacientes)
            {
                DbMockeada.GuardarPaciente(paciente);
            }
        }

        [When("Ingresa a urgencia el siguiente paciente:")]
        public void WhenIngresaAUrgenciaElSiguientePaciente(DataTable dataTable)
        {
            
        }

        [Then("La lista de espera esta ordenada por cuil de la siguiente manera:")]
        public void ThenLaListaDeEsperaEstaOrdenadaPorCuilDeLaSiguienteManera(DataTable dataTable)
        {
            throw new PendingStepException();
        }
    }
}
