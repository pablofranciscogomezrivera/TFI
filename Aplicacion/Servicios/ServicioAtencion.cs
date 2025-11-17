using Aplicacion.Intefaces;
using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion;

public class ServicioAtencion : IServicioAtencion
{
    public Atencion RegistrarAtencion(Ingreso ingreso, string informeMedico, Doctor doctor)
    {
        // Validar que el ingreso no sea nulo
        if (ingreso == null)
        {
            throw new ArgumentNullException(nameof(ingreso), "El ingreso es mandatorio");
        }

        // Validar que el doctor no sea nulo
        if (doctor == null)
        {
            throw new ArgumentNullException(nameof(doctor), "El médico es mandatorio");
        }

        // Validar que el informe no esté vacío
        if (string.IsNullOrWhiteSpace(informeMedico))
        {
            throw new ArgumentException("El informe del paciente es mandatorio");
        }

        // Validar que el ingreso esté en estado EN_PROCESO
        if (ingreso.Estado != EstadoIngreso.EN_PROCESO)
        {
            throw new InvalidOperationException("Solo se pueden registrar atenciones para ingresos en proceso");
        }

        // Validar que el ingreso tenga una atención
        if (ingreso.Atencion == null)
        {
            throw new InvalidOperationException("El ingreso no tiene una atención asociada");
        }

        // Actualizar el informe con el informe del médico
        // El informe original de la enfermera se mantiene, agregamos el informe del médico
        ingreso.Atencion.Informe = $"{ingreso.Atencion.Informe}\n\nInforme médico: {informeMedico}";

        // Asignar el doctor (si no estaba asignado)
        if (ingreso.Atencion.Doctor == null)
        {
            ingreso.Atencion.Doctor = doctor;
        }

        // Cambiar el estado del ingreso a FINALIZADO
        ingreso.Estado = EstadoIngreso.FINALIZADO;

        return ingreso.Atencion;
    }
}
