using Aplicacion.Intefaces;
using Dominio.Entidades;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion;

public class ServicioUrgencias : IServicioUrgencias
{
    private readonly IRepositorioPacientes _repositorioPacientes;
    private readonly IRepositorioUrgencias _repositorioUrgencias;

    public ServicioUrgencias(IRepositorioPacientes repositorioPacientes, IRepositorioUrgencias repositorioUrgencias)
    {
        _repositorioPacientes = repositorioPacientes;
        _repositorioUrgencias = repositorioUrgencias;
    }

    public List<Ingreso> ObtenerIngresosPendientes()
    {
        return _repositorioUrgencias.ObtenerIngresosPendientes();
    }

    public Ingreso ReclamarPaciente(Doctor doctor)
    {
        if (doctor == null)
        {
            throw new ArgumentNullException(nameof(doctor), "El doctor es requerido");
        }

        var listaEspera = _repositorioUrgencias.ObtenerIngresosPendientes();

        if (listaEspera.Count == 0)
        {
            throw new InvalidOperationException("No hay pacientes en la lista de espera");
        }

        // Obtener el primer paciente (el de mayor prioridad)
        var ingreso = listaEspera[0];

        // Cambiar el estado a EN_PROCESO
        ingreso.Estado = EstadoIngreso.EN_PROCESO;

        // Asignar el doctor a la atención
        ingreso.Atencion.Doctor = doctor;

        // Remover de la lista de espera
        _repositorioUrgencias.RemoverIngreso(ingreso);

        // Actualizar el ingreso
        _repositorioUrgencias.ActualizarIngreso(ingreso);

        return ingreso;
    }

    public void RegistrarUrgencia(string CUILPaciente, Enfermera Enfermera, string informe, double Temperatura, NivelEmergencia NivelEmergencia, double FrecCardiaca, double FrecRespiratoria, double FrecSistolica, double FrecDiastolica)
    {
        if (string.IsNullOrWhiteSpace(informe))
        {
            throw new ArgumentException("El informe es un dato mandatorio");
        }
        var paciente = _repositorioPacientes.BuscarPacientePorCuil(CUILPaciente);

        if (paciente is null)
        {
            var nuevoPaciente = new Paciente
            {
                CUIL = CUILPaciente
            };
            paciente = _repositorioPacientes.RegistrarPaciente(nuevoPaciente);
        }

        var ingreso = new Ingreso(
            paciente,
            Enfermera,
            informe,
            NivelEmergencia,
            Temperatura,
            FrecCardiaca,
            FrecRespiratoria,
            FrecSistolica,
            FrecDiastolica);

        _repositorioUrgencias.AgregarIngreso(ingreso);
    }
}
