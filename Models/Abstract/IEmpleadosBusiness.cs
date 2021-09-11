using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Abstract
{
    public interface IEmpleadosBusiness
    {

        Task<IEnumerable<Empleado>> ObtenerListaEmpleados();
        Task<Empleado> ObtenerEmpleadoPorId(int id);
        Task GuardarEmpleado(Empleado empleado);
        Task EditarEmpleado(Empleado empleado);
        Task EliminarEmpleado(Empleado empleado);

    }
}
