using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivTours.Models.Entities
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClienteId { get; set; }

        public string Nombre { get; set; }

        public string Apellidos { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.PhoneNumber)]
        public int Telefono { get; set; }

        [DataType(DataType.PhoneNumber)]
        public int TelefonoEmer { get; set; }
        public string Pais { get; set; }
        public int IdTipoDoc { get; set; }
        public int NumDoc { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaNac { get; set; }
        public string Notas { get; set; }
        public string Estado { get; set; }
    }
}
