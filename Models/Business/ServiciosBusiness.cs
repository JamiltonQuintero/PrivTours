using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using PrivTours.Models.DAL;
using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrivTours.Models.Business

{
    public class ServiciosBusiness : IServiciosBusiness
    {

        private readonly DbContextPriv _dbContext;

        public ServiciosBusiness(DbContextPriv context)
        {
            _dbContext = context;
        }

        public async Task<IEnumerable<Servicio>> ObtenerListaServicios()
        {
            return await _dbContext.Servicios.ToListAsync();
        }

        public async Task<Servicio> ObtenerServicioPorId(int id)
        {
            return await _dbContext.Servicios.FirstOrDefaultAsync(m => m.ServicioId == id);
        }
        public async Task GuardarServicio(Servicio servicio)
        {
            try
            {
                _dbContext.Add(servicio);
                await _dbContext.SaveChangesAsync();
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<DetallePermiso>> ObtenerPermisosPorRolId(string id)
        {
           var permisos = await _dbContext.DetallePermisos.ToListAsync();
            permisos.FindAll(dp => dp.RoleIdentityId == id);
            return permisos;
        }


        public async Task EditarServicio(Servicio servicio)
        {

            try
            {
                _dbContext.Update(servicio);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task EliminarServicio(Servicio servicio)
        {
            try
            {
                _dbContext.Remove(servicio);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
