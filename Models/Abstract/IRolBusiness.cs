using PrivTours.Models.Entities;
using PrivTours.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Abstract
{
    public interface IRolBusiness
    {
        Task<List<Permiso>> ObtenerListaPermisos();
        Task<List<RoleIdentity>> ObtenerListaRoles();
        Task GuardarDetallePermisos(RolViewModel rolViewModel);
        Task<List<DetallePermiso>> ObtenerPermisosPorRolId(string roleIdentityId);
        Task<bool> EliminarPermisosPorRolId(string roleIdentityId);
    }
}
