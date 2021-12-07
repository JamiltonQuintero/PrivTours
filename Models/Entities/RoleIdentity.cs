using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Entities
{
    public class RoleIdentity: IdentityRole
    {
        public string Descripcion { get; set; }
        public ICollection<DetallePermiso> DetallePermiso { get; set; }

    }
}
