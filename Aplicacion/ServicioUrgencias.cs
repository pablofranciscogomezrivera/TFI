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
    public IRepositorioPacientes _repositorio;

    public ServicioUrgencias(IRepositorioPacientes repositorio)
    {
        _repositorio = repositorio;
    }   


    public void RegistrarUrgencia(string CUILPaciente, Enfermera Enfermera, string Informe, double Temperatura, NivelEmergencia NivelEmergencia, FrecuenciaCardiaca FrecCardiaca, FrecuenciaRespiratoria FrecRespiratoria, double FrecSistolica, double FrecDiastolica)
    {
        throw new NotImplementedException();
    }
}

