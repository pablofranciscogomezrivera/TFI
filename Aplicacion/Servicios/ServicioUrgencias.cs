using Aplicacion.Intefaces;
using Dominio.Entidades;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Servicios;

public class ServicioUrgencias : IServicioUrgencias
{
    private readonly IRepositorioPacientes _repositorio;
    private readonly List<Ingreso> _listaEspera = new List<Ingreso>();

    public ServicioUrgencias(IRepositorioPacientes repositorio)
    {
        _repositorio = repositorio;
    }

    public List<Ingreso> ObtenerIngresosPendientes()
    {
        return _listaEspera;
    }

    public void RegistrarUrgencia(string CUILPaciente, Enfermera Enfermera, string informe, double Temperatura, NivelEmergencia NivelEmergencia, double FrecCardiaca, double FrecRespiratoria, double FrecSistolica, double FrecDiastolica)
    {
        if (string.IsNullOrWhiteSpace(informe))
        {
            throw new ArgumentException("El informe es un dato mandatorio");
        }
        var paciente = _repositorio.BuscarPacientePorCuil(CUILPaciente);

        if(paciente is null)
        {
            var nuevoPaciente = new Paciente
            {
                CUIL = CUILPaciente
            };
            paciente = _repositorio.RegistrarPaciente(nuevoPaciente);
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

        _listaEspera.Add(ingreso);
        _listaEspera.Sort();
    }

    public Ingreso ReclamarPaciente(Doctor doctor)
    {
        if (doctor == null)
        {
            throw new ArgumentNullException(nameof(doctor), "El doctor es requerido");
        }

        if (_listaEspera.Count == 0)
        {
            throw new InvalidOperationException("No hay pacientes en la lista de espera");
        }

        // Obtener el primer paciente (el de mayor prioridad)
        var ingreso = _listaEspera[0];

        // Cambiar el estado a EN_PROCESO
        ingreso.Estado = EstadoIngreso.EN_PROCESO;

        // Asignar el doctor a la atención
        ingreso.Atencion.Doctor = doctor;

        // Remover de la lista de espera
        _listaEspera.RemoveAt(0);

        return ingreso;
    }
}

