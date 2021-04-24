using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class SolicitudViewModel
    {

        public int SolicitudId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFinal { get; set; }
        public string Descripcion { get; set; }

        public int ClienteId { get; set; }
        public string NombreCliente { set; get; }

        public int EmpleadoId { get; set; }
        public string NombreEmpleado { set; get; }
         
        public int ServicioId { get; set; }
        public string NombreServicio{ set; get; }


    }
}
