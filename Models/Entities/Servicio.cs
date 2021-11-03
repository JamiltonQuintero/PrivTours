using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace PrivTours.Models.Entities
{
 
    public class Servicio
    {
 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServicioId { get; set; }

        [DisplayName("Nombre del servicio")]
        [Required(ErrorMessage = "{0} is required")]
        public string Nombre { get; set; }

        [DisplayName("Descripción del servicio")]
        [Required(ErrorMessage = "{0} is required")]
        public string Descripcion { set; get; }

        [DisplayName("Restricciones para servicio")]
        [Required(ErrorMessage = "{0} is required")]
        public string Restriccion { set; get; }

        [DisplayName("Recomendaciones para servicio")]
        [Required(ErrorMessage = "{0} is required")]
        public string Recomendacion { set; get; }
        public bool Estado { set; get; }
    }
}
