using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using PrivTours.Models.DAL;
using PrivTours.Models.Entities;
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
            return await _dbContext.Clientes.ToListAsync();
        }
     
        public async Task<IEnumerable<Servicio>> ObtenerListaServicios()
        {
            return await _dbContext.Servicios.ToListAsync();
        }

        public async Task<IEnumerable<Empleado>> ObtenerListaEmpleados()
        {
            return await _dbContext.Empleados.ToListAsync();
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
                    NombreCliente = c.Nombre
                    
                }
                ).ToListAsync();

                return  solicitudes;

        }

        public async Task GuardarSolicitud(Solicitud solicitud)
        {

            try
            {

                _dbContext.Add(solicitud);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception e) 
            {

                Console.WriteLine(e.InnerException.Message);

            }
        }

        

    }
}
