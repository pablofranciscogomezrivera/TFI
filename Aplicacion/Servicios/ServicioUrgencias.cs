using Aplicacion.Intefaces;
using Aplicacion.DTOs;
using Dominio.Entidades;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion;

public class ServicioUrgencias : IServicioUrgencias
{
    private readonly IRepositorioPacientes _repositorioPacientes;
    private readonly IRepositorioUrgencias _repositorioUrgencias;
    private readonly IRepositorioObraSocial _repositorioObraSocial;

    public ServicioUrgencias(
        IRepositorioPacientes repositorioPacientes,
        IRepositorioUrgencias repositorioUrgencias,
        IRepositorioObraSocial repositorioObraSocial)
    {
        _repositorioPacientes = repositorioPacientes;
        _repositorioUrgencias = repositorioUrgencias;
        _repositorioObraSocial = repositorioObraSocial;
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

        const int MaxRetries = 3;
        int retry = 0;

        while (retry < MaxRetries)
        {
            var candidato = _repositorioUrgencias.ObtenerSiguienteIngresoPendiente();

            if (candidato == null)
            {
                throw new InvalidOperationException("No hay pacientes en la lista de espera");
            }

            if (_repositorioUrgencias.IntentarAsignarMedico(candidato, doctor))
            {
                candidato.Estado = EstadoIngreso.EN_PROCESO;
                candidato.Atencion = new Atencion
                {
                    Doctor = doctor
                };
                return candidato;
            }

            retry++;
        }

        throw new Exception("El sistema está ocupado. Por favor, intente reclamar nuevamente.");
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

    public void RegistrarUrgencia(NuevaUrgenciaDto datos, Enfermera enfermera)
    {
        if (string.IsNullOrWhiteSpace(datos.Informe))
        {
            throw new ArgumentException("El informe es un dato mandatorio");
        }

        var ingresoEnCola = _repositorioUrgencias.BuscarIngresoPorCuilYEstado(datos.CuilPaciente, EstadoIngreso.PENDIENTE);

        if (ingresoEnCola != null)
        {
            throw new InvalidOperationException($"El paciente con CUIL {datos.CuilPaciente} ya se encuentra en la lista de espera y no puede ser ingresado nuevamente hasta que sea atendido o cancelado.");
        }

        var paciente = _repositorioPacientes.BuscarPacientePorCuil(datos.CuilPaciente);

        if (paciente is null)
        {
            var datosPaciente = datos.DatosPaciente;

            var domicilio = new Domicilio
            {
                Calle = datosPaciente?.Calle ?? "Sin Registrar",
                Numero = datosPaciente?.Numero ?? 999,
                Localidad = datosPaciente?.Localidad ?? "San Miguel de Tucumán"
            };

            Afiliado? nuevoAfiliado = null;
            if (datosPaciente?.ObraSocialId.HasValue == true && !string.IsNullOrWhiteSpace(datosPaciente.NumeroAfiliado))
            {
                var obraSocial = _repositorioObraSocial.BuscarObraSocialPorId(datosPaciente.ObraSocialId.Value);
                if (obraSocial != null)
                {
                    nuevoAfiliado = new Afiliado
                    {
                        ObraSocial = obraSocial,
                        NumeroAfiliado = datosPaciente.NumeroAfiliado
                    };
                }
            }

            paciente = new Paciente
            {
                CUIL = datos.CuilPaciente,
                DNI = ExtraerDniDeCuil(datos.CuilPaciente),
                Nombre = datosPaciente?.Nombre ?? "Sin Registrar",
                Apellido = datosPaciente?.Apellido ?? "Sin Registrar",
                FechaNacimiento = datosPaciente?.FechaNacimiento ?? new DateTime(1900, 1, 1),
                Email = datosPaciente?.Email ?? "",
                Telefono = datosPaciente?.Telefono ?? 0,
                Domicilio = domicilio,
                Afiliado = nuevoAfiliado
            };

            paciente = _repositorioPacientes.RegistrarPaciente(paciente);
        }

        var ingreso = new Ingreso(
            paciente,
            enfermera,
            datos.Informe,
            datos.NivelEmergencia,
            datos.Temperatura,
            datos.FrecuenciaCardiaca,
            datos.FrecuenciaRespiratoria,
            datos.FrecuenciaSistolica,
            datos.FrecuenciaDiastolica);

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
        return 0;
    }
}