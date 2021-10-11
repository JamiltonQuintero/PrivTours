using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Entities
{
    public class Empleado
    {

        public int EmpleadoId { get; set; }

        [DisplayName("Nombres")]
        [Required(ErrorMessage = "{0} is required")]
        public string Nombre { get; set; }

        [DisplayName("Apellidos")]
        [Required(ErrorMessage = "{0} is required")]
        public string Apellido { get; set; }

        [DisplayName("Celuar")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.PhoneNumber)]
        public string Celular { get; set; }
        public int TipoContrato { get; set; }
        public bool Estado { get; set; }

    }
}
