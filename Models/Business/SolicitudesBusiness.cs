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

        public async Task<List<Solicitud>> ObtenerListaSolicitudesSVM()
        {
            var solicitudes = await _dbContext.Solicitudes.ToListAsync();

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
        
        public async Task<List<SolicitudViewModel>> ObtenerListaSolicitudesPorEmpleado(string id)
        {
            List<SolicitudViewModel> solicitudes = new List<SolicitudViewModel>();
            var detalleSolicitudEmpleado = await _dbContext.DetalleSolicitudEmpleados.ToListAsync();         
            var empleadoUser = await _dbContext.UsuariosIdentity.FirstOrDefaultAsync(s => s.Id == id);
            foreach (DetalleSolicitudEmpleado d in detalleSolicitudEmpleado)
            {

                if (d.UsuarioIdentityId == id)
                {
                    var solicitud = await _dbContext.Solicitudes.FirstOrDefaultAsync(s => s.SolicitudId == d.SolicitudId);
                    if (solicitud != null)
                    {
                        var empleado = new Empleado
                        {
                            Nombre = empleadoUser.Nombre,
                            Apellido = empleadoUser.Apellido,
                        };
                        var svm = new SolicitudViewModel
                        {
                            SolicitudId = solicitud.SolicitudId,
                            FechaInicio = solicitud.FechaInicio,
                            FechaFin = solicitud.FechaFin,
                            Descripcion = solicitud.Descripcion,
                            HoraInicio = solicitud.HoraInicio,
                            HoraFinal = solicitud.HoraFinal,
                            Empleado = empleado
                        };
                        solicitudes.Add(svm);
                    }
                }
            } 

            return solicitudes;

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

        public async Task<HashSet<int>> ObtenerSolicitudesPorEmpleadosSeleccionados(string[] empleados)
        {

            List<SolicitudViewModel> solicitudes = new List<SolicitudViewModel>();
            HashSet<int> listaSolicitudes = new HashSet<int>();
            var detalleSolicitudEmpleado = await _dbContext.DetalleSolicitudEmpleados.ToListAsync();
            foreach (DetalleSolicitudEmpleado d in detalleSolicitudEmpleado)
            {
                foreach (string e in empleados)
                {
                    if (d.UsuarioIdentityId == e)
                    {
                        listaSolicitudes.Add(d.SolicitudId);
                    }
                }

            }

            return listaSolicitudes;

        }

            public async Task<Solicitud> ObtenerSolicitudPorId(int id)
        {
            return await _dbContext.Solicitudes.FirstOrDefaultAsync(s => s.SolicitudId == id);
        }

        public async Task<bool> EditarSolicitud(Solicitud solicitud, string[] empleados)
        {

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {

                    _dbContext.Update(solicitud);
                    await _dbContext.SaveChangesAsync();

                    foreach (string e in empleados)
                    {
                        DetalleSolicitudEmpleado detalleSolicitudEmpleado = new DetalleSolicitudEmpleado
                        {
                            SolicitudId = solicitud.SolicitudId,
                            UsuarioIdentityId = e
                        };
                        _dbContext.Add(detalleSolicitudEmpleado);
                    }
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                    return true;

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine(e.InnerException.Message);
                    return false;
                }
            }

        }

        public async Task<bool> EditarSolicitudEstado(Solicitud solicitud)
        {
            try
            {
                 _dbContext.Update(solicitud);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public async Task<bool> GuardarSolicitud(Solicitud solicitud, string[] empleados)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {

                    solicitud.EstadoSoliciud = (byte)EEstadoSolicitud.RESERVADO;
                    _dbContext.Add(solicitud);
                    await _dbContext.SaveChangesAsync();

                    foreach (string e in empleados)
                    {
                        DetalleSolicitudEmpleado detalleSolicitudEmpleado = new DetalleSolicitudEmpleado
                        {
                            SolicitudId = solicitud.SolicitudId,
                            UsuarioIdentityId = e
                        };
                        _dbContext.Add(detalleSolicitudEmpleado);
                    }
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                    return true;

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine(e.InnerException.Message);
                    return false;
                }
            }
        }

        public async Task<List<DetalleSolicitudEmpleado>> ObtenerDetalleEmpleadoPorSolicitudId(int solicitudId)
        {

               var detalle = await _dbContext.DetalleSolicitudEmpleados.ToListAsync();
                
                return detalle.FindAll(d => d.SolicitudId == solicitudId);
            
        }

        public async Task<bool> EliminarDetallesEmpleadosPorId(int id)
        {
            try
            {
                var detalle = await _dbContext.DetalleSolicitudEmpleados.ToListAsync();
                detalle = detalle.FindAll(d => d.SolicitudId == id);
                foreach (DetalleSolicitudEmpleado i in detalle)
                {
                    _dbContext.Remove(i);
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
