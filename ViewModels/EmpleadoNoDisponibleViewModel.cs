using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class EmpleadoNoDisponibleViewModel
    {

        public string Id { get; set; }
        [Required(ErrorMessage = "El nombre del rol es requerido")]
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        public string HoraInicio { get; set; }

        public string HoraFinal { get; set; }

    }
}
