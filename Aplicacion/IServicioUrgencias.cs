using Dominio.Entidades;

namespace Aplicacion;

public interface IServicioUrgencias
{
    public void RegistrarUrgencia(string CUILPaciente,
                                  Enfermera Enfermera,
                                  string Informe,
                                  double Temperatura,
                                  NivelEmergencia NivelEmergencia,
                                  FrecuenciaCardiaca FrecCardiaca,
                                  FrecuenciaRespiratoria FrecRespiratoria,
                                  double FrecSistolica,
                                  double FrecDiastolica);
}
