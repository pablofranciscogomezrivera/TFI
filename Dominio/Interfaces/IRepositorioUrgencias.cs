using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces;

public interface IRepositorioUrgencias
{
    void AgregarIngreso(Ingreso ingreso);
    List<Ingreso> ObtenerIngresosPendientes();
    void RemoverIngreso(Ingreso ingreso);
    void ActualizarIngreso(Ingreso ingreso);

    Ingreso? BuscarIngresoPorCuilYEstado(string cuil, EstadoIngreso estado);
    List<Ingreso> ObtenerTodosLosIngresos();
}