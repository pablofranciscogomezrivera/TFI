using Dominio.Entidades;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Memory;

public class RepositorioUrgenciasMemoria : IRepositorioUrgencias
{
    private readonly List<Ingreso> _ingresos = new List<Ingreso>();

    public void AgregarIngreso(Ingreso ingreso)
    {
        if (ingreso == null)
            throw new ArgumentNullException(nameof(ingreso));

        _ingresos.Add(ingreso);
        _ingresos.Sort(); // Mantener la lista ordenada por prioridad
    }

    public List<Ingreso> ObtenerIngresosPendientes()
    {
        // Retornar solo los ingresos que están en estado PENDIENTE
        return _ingresos.Where(i => i.Estado == EstadoIngreso.PENDIENTE).ToList();
    }

    public void RemoverIngreso(Ingreso ingreso)
    {
        if (ingreso == null)
            throw new ArgumentNullException(nameof(ingreso));

        _ingresos.Remove(ingreso);
    }

    public void ActualizarIngreso(Ingreso ingreso)
    {
        if (ingreso == null)
            throw new ArgumentNullException(nameof(ingreso));

        // No necesitamos hacer nada especial aquí porque trabajamos con referencias
        // El ingreso ya está modificado en memoria
        // Este método está para mantener la interfaz completa y por si en el futuro
        // necesitamos implementar un repositorio con persistencia real
    }
}
