﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Entities
{
    public class Solicitud
    {

        public int SolicitudId { get; set; }

        [DisplayName("Fecha de inicio")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha de fin")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Date)]
        public DateTime FechaFin {get; set; }

        [DisplayName("Hora de inicio")]
        [Required(ErrorMessage = "{0} is required")]
        public string HoraInicio { get; set; }

        [DisplayName("Hora de fin")]
        [Required(ErrorMessage = "{0} is required")]  
        public string HoraFinal { get; set; }

        [DisplayName("Descripción de de la solicitud")]
        [Required(ErrorMessage = "{0} is required")]
        public string Descripcion { get; set; }

        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        public int ServicioId { get; set; }
        public virtual Servicio Servicio { get; set; }

        public byte EstadoSoliciud { get; set; }
        public ICollection<DetalleSolicitudEmpleado> DetalleSolicitudEmpleado { get; set; }
    }
}
