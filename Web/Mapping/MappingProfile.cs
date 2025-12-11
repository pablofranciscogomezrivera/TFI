using AutoMapper;
using Dominio.Entidades;
using API.DTOs.Urgencias;
using API.DTOs.Pacientes;
using API.DTOs.Auth;
using Aplicacion.DTOs;

namespace API.Mapping;

/// <summary>
/// Perfil de configuración de AutoMapper para mapear entidades de dominio a DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Ingreso, IngresoResponse>()
            .ForMember(destino => destino.CuilPaciente,
                       opcion => opcion.MapFrom(origen => origen.Paciente.CUIL))
            .ForMember(destino => destino.NombrePaciente,
                       opcion => opcion.MapFrom(origen => origen.Paciente.Nombre))
            .ForMember(destino => destino.ApellidoPaciente,
                       opcion => opcion.MapFrom(origen => origen.Paciente.Apellido))
            .ForMember(destino => destino.InformeInicial,
                       opcion => opcion.MapFrom(origen => origen.InformeIngreso))
            .ForMember(destino => destino.SignosVitales,
                       opcion => opcion.MapFrom(origen => origen));

        CreateMap<Ingreso, SignosVitalesDto>()
            .ForMember(destino => destino.Temperatura,
                       opcion => opcion.MapFrom(origen => origen.Temperatura.Valor))
            .ForMember(destino => destino.FrecuenciaCardiaca,
                       opcion => opcion.MapFrom(origen => origen.FrecuenciaCardiaca.Valor))
            .ForMember(destino => destino.FrecuenciaRespiratoria,
                       opcion => opcion.MapFrom(origen => origen.FrecuenciaRespiratoria.Valor))
            .ForMember(destino => destino.TensionSistolica,
                       opcion => opcion.MapFrom(origen => origen.TensionArterial.FrecuenciaSistolica.Valor))
            .ForMember(destino => destino.TensionDiastolica,
                       opcion => opcion.MapFrom(origen => origen.TensionArterial.FrecuenciaDiastolica.Valor));

        CreateMap<Paciente, PacienteResponse>()
            .ForMember(destino => destino.Cuil,
                       opcion => opcion.MapFrom(origen => origen.CUIL));

        CreateMap<Domicilio, DomicilioDto>();

        CreateMap<Afiliado, AfiliadoDto>()
            .ForMember(destino => destino.ObraSocial,
                       opcion => opcion.MapFrom(origen => origen.ObraSocial.Nombre));

        CreateMap<Usuario, UsuarioResponse>();

        CreateMap<RegistrarUrgenciaRequest, NuevaUrgenciaDto>()
            .ForMember(destino => destino.DatosPaciente,
                       opcion => opcion.MapFrom(origen => new DatosNuevoPacienteDto
                       {
                           Nombre = origen.NombrePaciente,
                           Apellido = origen.ApellidoPaciente,
                           Calle = origen.CallePaciente,
                           Numero = origen.NumeroPaciente,
                           Localidad = origen.LocalidadPaciente,
                           Email = origen.EmailPaciente,
                           Telefono = origen.TelefonoPaciente,
                           ObraSocialId = origen.ObraSocialIdPaciente,
                           NumeroAfiliado = origen.NumeroAfiliadoPaciente,
                           FechaNacimiento = origen.FechaNacimientoPaciente
                       }));
    }
}
