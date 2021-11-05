﻿using PrivTours.Models.Entities;
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
        Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorEmpleado(string id);
        Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorCliente(int clienteId);
        Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorServicio(int servicioId);
        Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorEstado(byte estado);
        Task<HashSet<int>> ObtenerSolicitudesPorEmpleadosSeleccionados(string[] empleados);
        Task<List<Solicitud>> ObtenerListaSolicitudesPorEmpleadoTareas(string id);
        Task<List<Solicitud>> ObtenerListaSolicitudesSVM();
        Task<IEnumerable<Empleado>> ObtenerListaEmpleados();
        Task<IEnumerable<Servicio>> ObtenerListaServicios();
        Task<IEnumerable<Cliente>> ObtenerListaClientes();
        Task<bool> EliminarDetallesEmpleadosPorId(int id);
        Task<bool> GuardarSolicitud(Solicitud solicitud, string[] empleados);
        Task<Solicitud> ObtenerSolicitudPorId(int id);
        Task<bool> EditarSolicitud(Solicitud solicitud, string[] empleados);
        Task<bool> EditarSolicitudEstado(Solicitud solicitud);
        Task<List<DetalleSolicitudEmpleado>> ObtenerDetalleEmpleadoPorSolicitudId(int solicitudId);

    }
}
