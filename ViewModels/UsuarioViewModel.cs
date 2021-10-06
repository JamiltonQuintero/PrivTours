using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Threading.Tasks;
using System;

namespace PrivTours.ViewModels
{
    public class UsuarioViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Utilice caracteres solamente")]
        [StringLength(25, ErrorMessage = "El {0} debe tener al menos {2} y maximo {1} caracteres.", MinimumLength = 2)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Utilice caracteres solamente")]
        [StringLength(25, ErrorMessage = "El {0} debe tener al menos {2} y maximo {1} caracteres.", MinimumLength = 2)]
        public string Apellido { get; set; }

        [DisplayName("Documento")]
        [Required(ErrorMessage = "El documento es requerido")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "El documento debe contener solo números")]
        [StringLength(25, ErrorMessage = "El {0} debe tener al menos {2} y maximo {1} caracteres.", MinimumLength = 5)]

        public string Documento { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Correo invalido")]
        public string Email { get; set; }
        public string Telefono { get; set; }


        [Required(ErrorMessage = "La contraseña es requerida")]
        [DisplayName("Contraseña")]
        [DataType(DataType.Password)]
        [StringLength(16, ErrorMessage = "El {0} debe tener al menos {2} y maximo {1} caracteres.", MinimumLength = 8)]
        public string Password { get; set; }


        [Required(ErrorMessage = "La contraseña es requerida")]
        [DisplayName("Confirmar Contraseña")]
        [DataType(DataType.Password)]
        [StringLength(16, ErrorMessage = "El {0} debe tener al menos {2} y maximo {1} caracteres.", MinimumLength = 8)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarPassword { get; set; }

        public List<string> Rol { get; set; }
        public string RolSeleccionado { get; set; }

        public Boolean Estado { get; set; }

    }
}
