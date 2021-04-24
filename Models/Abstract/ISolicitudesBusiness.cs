using PrivTours.Models.Entities;
using PrivTours.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Abstract
{
    public interface ISolicitudesBusiness
    {

        Task<List<SolicitudViewModel>> ObtenerListaSolicitudes();
        Task<IEnumerable<Empleado>> ObtenerListaEmpleados();
        Task<IEnumerable<Servicio>> ObtenerListaServicios();
        Task<IEnumerable<Cliente>> ObtenerListaClientes();
        Task GuardarSolicitud(Solicitud solicitud);

    }
}
