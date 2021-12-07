using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class RolViewModel 
    {
        public string Id { get; set; }
    
        [DisplayName("Nombre Rol")]
        public string NombreRol { get; set; }
        public string[] Permisos { get; set; }
        public string[] PermisosSinSelec { get; set; }

        public string RoleIdentityId { get; set; }
        public RoleIdentity RoleIdentity { get; set; }
        public List<DetallePermiso> DetallePermiso { get; set; }
        public string Descripcion { get; set; }

        public string TextoPermisos { get; set; }
    }
}
