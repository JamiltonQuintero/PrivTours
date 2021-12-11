using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PrivTours.Models.Entities
{
    public class UsuarioIdentity: IdentityUser
    {
        [Required(ErrorMessage = "{0} is required")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Documento { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Telefono { get; set; }
        public int TipoContrato { get; set; }
    }
}
