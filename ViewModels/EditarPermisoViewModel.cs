using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class EditarPermisoViewModel
    {
        public string RoleIdentityId { get; set; }
        public List<Permiso> PermisosSeleccionados { get; set; }
        public List<Permiso> Permisos { get; set; }

    }
}
