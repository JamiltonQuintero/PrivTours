using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Entities
{
    public class DetalleSolicitudEmpleado
    {

        [Key]
        public int DetalleSolicitudEmpleadoId { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public int SolicitudId { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string UsuarioIdentityId { get; set; }
        public Solicitud Solicitud { get; set; }
        public UsuarioIdentity UsuarioIdentity { get; set; }

    }
}
