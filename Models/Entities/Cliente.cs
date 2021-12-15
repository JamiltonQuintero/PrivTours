using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace PrivTours.Models.Entities
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClienteId { get; set; }

        [DisplayName("Nombres")]
        [Required(ErrorMessage = "Debes ingresar un nombre.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debes ingresar un apellido.")]

        public string Apellidos { get; set; }

        [DisplayName("Correo electrónico")]
        [Required(ErrorMessage = "Debes ingresar un email.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Teléfono")]
        [Required(ErrorMessage = "El teléfono es requerido.")]
        [DataType(DataType.PhoneNumber)]
        public int Telefono { get; set; }

        [DisplayName("Teléfono de emergencia")]
        [Required(ErrorMessage = "El teléfono es requerido.")]
        [DataType(DataType.PhoneNumber)]
        public int TelefonoEmer { get; set; }

        [DisplayName("País")]
        public string Pais { get; set; }
        [DisplayName("Tipo de documento")]
        public int IdTipoDoc { get; set; }

        [DisplayName("Número de documento")]
        public string NumDoc { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Fecha de nacimiento")]
        public DateTime FechaNac { get; set; }

        [DisplayName("Notas especial")]
        public string Notas { get; set; }
        public bool Estado { get; set; }
    }
}
