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
        var lista = _repositorioUrgencias.ObtenerIngresosPendientes();
        lista.Sort();
        return lista;
    }

    public List<Ingreso> ObtenerTodosLosIngresos()
    {
        return _repositorioUrgencias.ObtenerTodosLosIngresos();
    }

    public Ingreso ReclamarPaciente(Doctor doctor)
    {
        if (doctor == null)
        {
            throw new ArgumentNullException(nameof(doctor), "El doctor es requerido");
        }

        var listaEspera = _repositorioUrgencias.ObtenerIngresosPendientes();
        listaEspera.Sort();

        if (listaEspera.Count == 0)
        {
            throw new InvalidOperationException("No hay pacientes en la lista de espera");
        }

        var ingreso = listaEspera[0];

        ingreso.Estado = EstadoIngreso.EN_PROCESO;

        _repositorioUrgencias.ActualizarIngreso(ingreso);

        return ingreso;
    }

    public void CancelarAtencion(string cuilPaciente)
    {
        if (string.IsNullOrWhiteSpace(cuilPaciente))
        {
            throw new ArgumentException("El CUIL del paciente es mandatorio");
        }

        var ingreso = _repositorioUrgencias.BuscarIngresoPorCuilYEstado(cuilPaciente, EstadoIngreso.EN_PROCESO);

        if (ingreso == null)
        {
            throw new InvalidOperationException($"No se encontró un ingreso en proceso para el paciente con CUIL {cuilPaciente}");
        }

        ingreso.Estado = EstadoIngreso.PENDIENTE;

        _repositorioUrgencias.ActualizarIngreso(ingreso);
    }

    public void RegistrarUrgencia(
        string CUILPaciente,
        Enfermera Enfermera,
        string informe,
        double Temperatura,
        NivelEmergencia NivelEmergencia,
        double FrecCardiaca,
        double FrecRespiratoria,
        double FrecSistolica,
        double FrecDiastolica,
        // Parámetros opcionales para crear paciente si no existe
        string? nombrePaciente = null,
        string? apellidoPaciente = null,
        string? callePaciente = null,
        int? numeroPaciente = null,
        string? localidadPaciente = null,
        string? emailPaciente = null,
        long? telefonoPaciente = null,
        int? obraSocialIdPaciente = null,
        string? numeroAfiliadoPaciente = null,
        DateTime? fechaNacimientoPaciente = null)
    {
        if (string.IsNullOrWhiteSpace(informe))
        {
            throw new ArgumentException("El informe es un dato mandatorio");
        }

        var paciente = _repositorioPacientes.BuscarPacientePorCuil(CUILPaciente);

        if (paciente is null)
        {
            var domicilio = new Domicilio
            {
                Calle = callePaciente ?? "Sin Registrar",
                Numero = numeroPaciente ?? 999,
                Localidad = localidadPaciente ?? "San Miguel de Tucumán"
            };

            paciente = new Paciente
            {
                CUIL = CUILPaciente,
                DNI = ExtraerDniDeCuil(CUILPaciente),
                Nombre = nombrePaciente ?? "Sin Registrar",
                Apellido = apellidoPaciente ?? "Sin Registrar",
                FechaNacimiento = fechaNacimientoPaciente ?? new DateTime(1900, 1, 1),
                Email = emailPaciente ?? "",
                Telefono = telefonoPaciente ?? 0,
                Domicilio = domicilio,
                Afiliado = null // Sin obra social por defecto
            };

            paciente = _repositorioPacientes.RegistrarPaciente(paciente);
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

    private int ExtraerDniDeCuil(string cuil)
    {
        var cuilLimpio = cuil.Replace("-", "");
        if (cuilLimpio.Length >= 10)
        {
            var dniString = cuilLimpio.Substring(2, 8);
            if (int.TryParse(dniString, out int dni))
            {
                return dni;
            }
        }
        return 0; // Valor por defecto si no se puede extraer
    }
}
