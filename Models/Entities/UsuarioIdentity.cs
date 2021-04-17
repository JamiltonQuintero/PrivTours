using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Entities
{
    public class UsuarioIdentity : IdentityUser
    {

        public string Nombre { get; set; }      

    }
}
