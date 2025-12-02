using Aplicacion.Intefaces;
using Dominio.Entidades;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion;

public class ServicioAtencion : IServicioAtencion
{
    private readonly IRepositorioUrgencias _repositorioUrgencias;

    public ServicioAtencion(IRepositorioUrgencias repositorioUrgencias)
    {
        _repositorioUrgencias = repositorioUrgencias ?? throw new ArgumentNullException(nameof(repositorioUrgencias));
    }

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
            throw new ArgumentException("El informe médico es mandatorio");
        }

        // Validar que el ingreso esté en estado EN_PROCESO
        if (ingreso.Estado != EstadoIngreso.EN_PROCESO)
        {
            throw new InvalidOperationException("Solo se pueden registrar atenciones para ingresos en proceso");
        }

        // Crear la atención médica
        ingreso.Atencion = new Atencion
        {
            Doctor = doctor,
            Informe = informeMedico
        };

        // Cambiar el estado del ingreso a FINALIZADO
        ingreso.Estado = EstadoIngreso.FINALIZADO;

        // Persistir los cambios en el repositorio
        _repositorioUrgencias.ActualizarIngreso(ingreso);

        return ingreso.Atencion;
    }
}
