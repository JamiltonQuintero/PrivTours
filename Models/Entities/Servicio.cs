using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivTours.Models.Entities
{
    [Table("Servicios")]
    public class Servicio
    {
 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServicioId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { set; get; }
        public string Restriccion { set; get; }
        public string Recomendacion { set; get; }
        public string Estado { set; get; }



    }
}
