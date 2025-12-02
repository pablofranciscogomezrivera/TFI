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
        // Validar campos mandatorios
        ValidarCamposMandatorios(cuil, nombre, apellido, calle, numero, localidad);

        // Validar formato de CUIL
        if (!ValidadorCUIL.EsValido(cuil))
        {
            throw new ArgumentException("El CUIL no tiene un formato válido");
        }

        // Validar obra social si se proporciona
        Afiliado? afiliado = null;
        if (obraSocialId.HasValue)
        {
            afiliado = ValidarYCrearAfiliado(obraSocialId.Value, numeroAfiliado);
        }

        // Crear el domicilio
        var domicilio = new Domicilio
        {
            Calle = calle,
            Numero = numero,
            Localidad = localidad
        };

        // Crear el paciente
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

        // Registrar el paciente en el repositorio
        return _repositorioPacientes.RegistrarPaciente(paciente);
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
        // Validar que el número de afiliado no esté vacío
        if (string.IsNullOrWhiteSpace(numeroAfiliado))
        {
            throw new ArgumentException("El número de afiliado es mandatorio cuando se especifica una obra social");
        }

        // Validar que la obra social exista
        if (!_repositorioObraSocial.ExisteObraSocial(obraSocialId))
        {
            throw new ArgumentException("No se puede registrar al paciente con una obra social inexistente");
        }

        // Validar que el paciente esté afiliado a la obra social
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
