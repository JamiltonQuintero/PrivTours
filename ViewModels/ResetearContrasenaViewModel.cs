using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class ResetearContrasenaViewModel
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email invalido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmar contraseña es requerido")]
        [Compare("Password", ErrorMessage = "Contraseña y confirmar contraseña no son iguales")]
        public string PasswordConfirm { get; set; }

        public string Code { get; set; }
    }
}
