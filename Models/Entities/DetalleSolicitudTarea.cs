using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Entities
{
    public class DetalleSolicitudTarea
    {

        [Key]
        public int DetalleSolicitudTareaId { get; set; }
        public int SolicitudId { get; set; }
        public string TareaId { get; set; }
        public Solicitud Solicitud { get; set; }
        public Tarea Tarea { get; set; }

    }
}
