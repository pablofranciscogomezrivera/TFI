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
        if (ingreso == null)
        {
            throw new ArgumentNullException(nameof(ingreso), "El ingreso es mandatorio");
        }

        if (doctor == null)
        {
            throw new ArgumentNullException(nameof(doctor), "El médico es mandatorio");
        }

        if (string.IsNullOrWhiteSpace(informeMedico))
        {
            throw new ArgumentException("El informe médico es mandatorio");
        }

        if (ingreso.Estado != EstadoIngreso.EN_PROCESO)
        {
            throw new InvalidOperationException("Solo se pueden registrar atenciones para ingresos en proceso");
        }

        ingreso.Atencion = new Atencion
        {
            Doctor = doctor,
            Informe = informeMedico
        };

        ingreso.Estado = EstadoIngreso.FINALIZADO;

        _repositorioUrgencias.ActualizarIngreso(ingreso);

        return ingreso.Atencion;
    }

    public Atencion RegistrarAtencionPorCuil(string cuilPaciente, string informeMedico, Doctor doctor)
    {
        if (string.IsNullOrWhiteSpace(cuilPaciente))
        {
            throw new ArgumentException("El CUIL del paciente es mandatorio");
        }

        if (doctor == null)
        {
            throw new ArgumentNullException(nameof(doctor), "El médico es mandatorio");
        }

        if (string.IsNullOrWhiteSpace(informeMedico))
        {
            throw new ArgumentException("El informe médico es mandatorio");
        }

        var ingreso = _repositorioUrgencias.BuscarIngresoPorCuilYEstado(cuilPaciente, EstadoIngreso.EN_PROCESO);

        if (ingreso == null)
        {
            throw new InvalidOperationException($"No se encontró un ingreso en proceso para el paciente con CUIL {cuilPaciente}");
        }

        return RegistrarAtencion(ingreso, informeMedico, doctor);
    }
}
