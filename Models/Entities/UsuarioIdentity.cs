﻿using Microsoft.AspNetCore.Identity;


namespace PrivTours.Models.Entities
{
    public class UsuarioIdentity: IdentityUser
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Documento { get; set; }
        public string Telefono { get; set; }

    }
}
