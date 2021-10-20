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
        Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorCliente(int clienteId);
        Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorEmpleado(int empleadoId);
        Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorServicio(int servicioId);
        Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorEstado(byte estado);
        Task<IEnumerable<Empleado>> ObtenerListaEmpleados();
        Task<IEnumerable<Servicio>> ObtenerListaServicios();
        Task<IEnumerable<Cliente>> ObtenerListaClientes();
        Task<bool> GuardarSolicitud(Solicitud solicitud);
        Task<Solicitud> ObtenerSolicitudPorId(int id);
        Task<bool> EditarSolicitud(Solicitud solicitud);

    }
}
