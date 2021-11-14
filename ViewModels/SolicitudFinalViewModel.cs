using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class SolicitudFinalViewModel
    {
   
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public string Descripcion { get; set; }

        public int ClienteId { get; set; }

        public int ServicioId { get; set; }
        public Array Tareas { get; set;}

    }
}
