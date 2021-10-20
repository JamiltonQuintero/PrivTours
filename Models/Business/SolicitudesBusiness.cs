using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using PrivTours.Models.DAL;
using PrivTours.Models.Entities;
using PrivTours.Models.Enums;
using PrivTours.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Business
{
    public class SolicitudesBusiness : ISolicitudesBusiness
    {


        private readonly DbContextPriv _dbContext;

        public SolicitudesBusiness(DbContextPriv dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Cliente>> ObtenerListaClientes()
        {

            var clientes = await _dbContext.Clientes.ToListAsync();

            var clientesActivos = clientes.FindAll(clientes => clientes.Estado == true);

            return clientesActivos;
        }
     
        public async Task<IEnumerable<Servicio>> ObtenerListaServicios()
        {
            var servicios = await _dbContext.Servicios.ToListAsync();

            var serviciosActivos = servicios.FindAll(servicios => servicios.Estado == true);

            return serviciosActivos;
        }

        public async Task<IEnumerable<Empleado>> ObtenerListaEmpleados()
        {
            var empleados = await _dbContext.Empleados.ToListAsync();

            var empleadosActivos = empleados.FindAll(empleado => empleado.Estado == true);

            return empleadosActivos;
        }


        public async Task<List<SolicitudViewModel>> ObtenerListaSolicitudes()
        {

            List<SolicitudViewModel> solicitudes = await _dbContext.Solicitudes.Join(
                _dbContext.Clientes,
                c => c.ClienteId,
                s => s.ClienteId,
                (s, c) => new SolicitudViewModel
                {
                    SolicitudId = s.SolicitudId,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    Descripcion = s.Descripcion,
                    HoraInicio = s.HoraInicio,
                    HoraFinal = s.HoraFinal,
                    ClienteId = c.ClienteId,
                    Cliente = c
                }
                ).ToListAsync();

            return solicitudes;

        }
        public async Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorCliente(int clienteId)
        {

            List<SolicitudViewModel> solicitudes = await _dbContext.Solicitudes.Join(
                _dbContext.Clientes,
                c => c.ClienteId,
                s => s.ClienteId,
                (s, c) => new SolicitudViewModel
                {
                    SolicitudId = s.SolicitudId,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    Descripcion = s.Descripcion,
                    HoraInicio = s.HoraInicio,
                    HoraFinal = s.HoraFinal,
                    ClienteId = c.ClienteId,
                    Cliente = c
                }
                ).ToListAsync();

                var solicitudesPorCliente = solicitudes.FindAll(cliente => cliente.ClienteId == clienteId);

                return  solicitudesPorCliente;

        }

        public async Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorEmpleado(int empleadoId)
        {

            List<SolicitudViewModel> solicitudes = await _dbContext.Solicitudes.Join(
                _dbContext.Empleados,
                c => c.EmpleadoId,
                e => e.EmpleadoId,
                (s, e) => new SolicitudViewModel
                {
                    SolicitudId = s.SolicitudId,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    Descripcion = s.Descripcion,
                    HoraInicio = s.HoraInicio,
                    HoraFinal = s.HoraFinal,
                    EmpleadoId = e.EmpleadoId,
                    Empleado = e
                }
                ).ToListAsync();

            var solicitudesPorEmpleado = solicitudes.FindAll(empleado => empleado.EmpleadoId == empleadoId);

            return solicitudesPorEmpleado;

        }

        public async Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorServicio(int servicioId)
        {

            List<SolicitudViewModel> solicitudes = await _dbContext.Solicitudes.Join(
                _dbContext.Servicios,
                se => se.ServicioId,
                s => s.ServicioId,
                (s, se) => new SolicitudViewModel
                {
                    SolicitudId = s.SolicitudId,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    Descripcion = s.Descripcion,
                    HoraInicio = s.HoraInicio,
                    HoraFinal = s.HoraFinal,
                    ServicioId = se.ServicioId,
                    Servicio = se
                }
                ).ToListAsync();

            var solicitudesPorServicio = solicitudes.FindAll(servicio => servicio.ServicioId == servicioId);

            return solicitudesPorServicio;

        }

        public async Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorEstado(byte estado)
        {
            List<SolicitudViewModel> solicitudes = await _dbContext.Solicitudes.Join(
                _dbContext.Clientes,
                c => c.ClienteId,
                s => s.ClienteId,
                (s, c) => new SolicitudViewModel
                {
                    SolicitudId = s.SolicitudId,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    Descripcion = s.Descripcion,
                    HoraInicio = s.HoraInicio,
                    HoraFinal = s.HoraFinal,
                    EstadoSoliciud = s.EstadoSoliciud,
                    ClienteId = c.ClienteId,
                    Cliente = c
                }
                ).ToListAsync();

            var solicitudesPorEstado = solicitudes.FindAll(solicitud => solicitud.EstadoSoliciud == estado);
            return solicitudesPorEstado;
        }


        public async Task<bool> GuardarSolicitud(Solicitud solicitud)
        {

            try
            {
                solicitud.EstadoSoliciud = (byte)EEstadoSolicitud.RESERVADO;
                _dbContext.Add(solicitud);
                await _dbContext.SaveChangesAsync();
                return true;

            }
            catch (Exception e) 
            {

                Console.WriteLine(e.InnerException.Message);
                return false;
            }
        }


        public async Task<Solicitud> ObtenerSolicitudPorId(int id)
        {
            return await _dbContext.Solicitudes.FirstOrDefaultAsync(s => s.SolicitudId == id);
        }

        public async Task<bool> EditarSolicitud(Solicitud solicitud)
        {

            try
            {
                _dbContext.Update(solicitud);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return false;
            }
        }

    }
}
