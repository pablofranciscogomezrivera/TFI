using Dominio.Entidades;

namespace Aplicacion.Intefaces;

public interface IServicioUrgencias
{
    public void RegistrarUrgencia(
        string CUILPaciente,
        Enfermera enfermera,
        string informe,
        double temperatura,
        NivelEmergencia nivelEmergencia,
        double frecCardiaca,
        double frecRespiratoria,
        double frecSistolica,
        double frecDiastolica,
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
        DateTime? fechaNacimientoPaciente = null);

    public List<Ingreso> ObtenerIngresosPendientes();
    public List<Ingreso> ObtenerTodosLosIngresos();
    public Ingreso ReclamarPaciente(Doctor doctor);
    public void CancelarAtencion(string cuilPaciente);
}
