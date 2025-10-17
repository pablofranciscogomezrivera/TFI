using Dominio.Entidades;

namespace Aplicacion;

public interface IServicioUrgencias
{
    public void RegistrarUrgencia(string CUILPaciente,
                                  Enfermera enfermera,
                                  string informe,
                                  double temperatura,
                                  NivelEmergencia nivelEmergencia,
                                  double frecCardiaca,
                                  double frecRespiratoria,
                                  double frecSistolica,
                                  double frecDiastolica);

    public List<Ingreso> ObtenerIngresosPendientes();
}
