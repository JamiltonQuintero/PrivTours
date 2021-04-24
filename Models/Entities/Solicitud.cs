using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Entities
{
    public class Solicitud
    {

        public int SolicitudId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin {get; set; }
        public string HoraInicio { get; set; }
        public string HoraFinal { get; set; }
        public string Descripcion { get; set; }

        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        public int EmpleadoId { get; set; }
        public virtual Empleado Empleado { get; set; }

        public int ServicioId { get; set; }
        public virtual Servicio Servicio { get; set; }

    }
}
