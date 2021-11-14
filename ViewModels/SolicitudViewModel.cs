using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class SolicitudViewModel
    {

        public int SolicitudId { get; set; }
        [DisplayName("Fecha de inicio")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [DisplayName("Fecha de fin")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [DisplayName("Descripción de de la solicitud")]
        [Required(ErrorMessage = "{0} is required")]
        public string Descripcion { get; set; }

        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        public int EmpleadoId { get; set; }
        public virtual Empleado Empleado { get; set; }

        public string[] Empleados { get; set; }
        public string EmpleadosNombres { get; set; }
        public int ServicioId { get; set; }
        public virtual Servicio Servicio { get; set; }

        public byte EstadoSoliciud { get; set; }

        public string Rol { get; set; }

        public int TareaId { get; set; }
        [DisplayName("Fecha de fin tarea")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Date)]
        public DateTime FechaInicioTarea { get; set; }
        [DisplayName("Fecha de fin tarea")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Date)]
        public DateTime FechaFinTarea { get; set; }
        [DisplayName("Descripción tarea")]
        [Required(ErrorMessage = "{0} is required")]
        public string DescripcionTarea { get; set; }

        public string UsuarioIdentityId { get; set; }
        [DisplayName("Empleado")]
        public virtual UsuarioIdentity UsuarioIdentity { get; set; }

        public int OperacionId { get; set; }
        public virtual Operacion Operacion { get; set; }

        public List<TareaViewModel> Tareas { get; set;}

    }
}
