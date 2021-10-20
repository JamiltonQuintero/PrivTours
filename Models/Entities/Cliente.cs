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
        public string Nombre { get; set; }

        public string Apellidos { get; set; }

        [DisplayName("Correo electronico")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Telefono")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.PhoneNumber)]
        public int Telefono { get; set; }

        [DisplayName("Telefono de emergencia")]
        [DataType(DataType.PhoneNumber)]
        public int TelefonoEmer { get; set; }
        public string Pais { get; set; }
        [DisplayName("Tipo de documento")]
        public int IdTipoDoc { get; set; }

        [DisplayName("Numero de documento")]
        public string NumDoc { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Fecha de nacimiento")]
        public DateTime FechaNac { get; set; }

        [DisplayName("Notas especial")]
        public string Notas { get; set; }
        public bool Estado { get; set; }
    }
}
