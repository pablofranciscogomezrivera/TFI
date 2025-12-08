using Aplicacion.Intefaces;
using Dominio.Entidades;
using Dominio.Interfaces;
using Dominio.Validadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion;

public class ServicioPacientes : IServicioPacientes
{
    private readonly IRepositorioPacientes _repositorioPacientes;
    private readonly IRepositorioObraSocial _repositorioObraSocial;

    public ServicioPacientes(
        IRepositorioPacientes repositorioPacientes,
        IRepositorioObraSocial repositorioObraSocial)
    {
        _repositorioPacientes = repositorioPacientes;
        _repositorioObraSocial = repositorioObraSocial;
    }

    public Paciente RegistrarPaciente(
        string cuil,
        string nombre,
        string apellido,
        string calle,
        int numero,
        string localidad,
        DateTime fechaNacimiento,
        int? obraSocialId = null,
        string? numeroAfiliado = null)
    {
        ValidarCamposMandatorios(cuil, nombre, apellido, calle, numero, localidad);

        Afiliado? afiliado = null;
        if (obraSocialId.HasValue)
        {
            afiliado = ValidarYCrearAfiliado(obraSocialId.Value, numeroAfiliado);
        }

        var domicilio = new Domicilio
        {
            Calle = calle,
            Numero = numero,
            Localidad = localidad
        };

        var paciente = new Paciente
        {
            CUIL = cuil,
            DNI = int.Parse(cuil.Substring(3, 8)),
            Nombre = nombre,
            Apellido = apellido,
            FechaNacimiento = fechaNacimiento,
            Domicilio = domicilio,
            Afiliado = afiliado
        };

        return _repositorioPacientes.RegistrarPaciente(paciente);
    }
    public Paciente? BuscarPacientePorCuil(string cuil)
    {
        return _repositorioPacientes.BuscarPacientePorCuil(cuil);
    }
    private void ValidarCamposMandatorios(
        string cuil,
        string nombre,
        string apellido,
        string calle,
        int numero,
        string localidad)
    {
        if (string.IsNullOrWhiteSpace(cuil))
            throw new ArgumentException("El CUIL es un campo mandatorio");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El Nombre es un campo mandatorio");

        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException("El Apellido es un campo mandatorio");

        if (string.IsNullOrWhiteSpace(calle))
            throw new ArgumentException("La Calle es un campo mandatorio");

        if (numero <= 0)
            throw new ArgumentException("El Numero debe ser mayor a 0");

        if (string.IsNullOrWhiteSpace(localidad))
            throw new ArgumentException("La Localidad es un campo mandatorio");
    }

    private Afiliado ValidarYCrearAfiliado(int obraSocialId, string? numeroAfiliado)
    {
        if (string.IsNullOrWhiteSpace(numeroAfiliado))
        {
            throw new ArgumentException("El número de afiliado es mandatorio cuando se especifica una obra social");
        }

        if (!_repositorioObraSocial.ExisteObraSocial(obraSocialId))
        {
            throw new ArgumentException("No se puede registrar al paciente con una obra social inexistente");
        }

        if (!_repositorioObraSocial.EstaAfiliadoAObraSocial(obraSocialId, numeroAfiliado))
        {
            throw new ArgumentException("No se puede registrar el paciente dado que no está afiliado a la obra social");
        }

        var obraSocial = _repositorioObraSocial.BuscarObraSocialPorId(obraSocialId);

        return new Afiliado
        {
            ObraSocial = obraSocial,
            NumeroAfiliado = numeroAfiliado
        };
    }
}
