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
}

