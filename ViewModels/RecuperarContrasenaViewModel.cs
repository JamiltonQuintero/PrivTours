using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class RecuperarContrasenaViewModel
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email invalido")]
        public string Email { get; set; }
    }
}
