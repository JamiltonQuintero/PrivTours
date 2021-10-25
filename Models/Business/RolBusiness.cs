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
    public class RolBusiness : IRolBusiness
    {
        private readonly DbContextPriv _dbContext;

        public RolBusiness(DbContextPriv context)
        {
            _dbContext = context;
        }

        public async Task GuardarDetallePermisos(RolViewModel rolViewModel)
        {
            try
            {
                foreach (string d in rolViewModel.Permisos)
                {
                    DetallePermiso detalle = new DetallePermiso
                    {
                        RoleIdentityId = rolViewModel.RoleIdentityId,
                        PermisoId = int.Parse(d)
                    };
                    _dbContext.Add(detalle);
                }

                await _dbContext.SaveChangesAsync();

            }
            catch(Exception e)
            {
                
            }
        }

        public async Task<List<Permiso>> ObtenerListaPermisos()
        {
            var permisos = await _dbContext.Permisos.ToListAsync();

            return permisos;
        }

        public async Task<List<RoleIdentity>> ObtenerListaRoles()
        {
            var roles = await _dbContext.RolesIdentity.ToListAsync();

            return roles;
        }

        public async Task<List<DetallePermiso>> ObtenerPermisosPorRolId(string roleIdentityId)
        {
            var permisos = await _dbContext.DetallePermisos.ToListAsync();

            return permisos = permisos.FindAll(d => d.RoleIdentityId == roleIdentityId);
        }

        public async Task<bool> EliminarPermisosPorRolId(string roleIdentityId)
        {
            try
            {
                var permisos = await ObtenerPermisosPorRolId(roleIdentityId);
                foreach(DetallePermiso i in permisos)
                {
                    _dbContext.Remove(i);
                }       
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
           
        }

    }
}
