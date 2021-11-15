using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Entities
{
    public class Tarea
    {

        public int TareaId { get; set; }

        public DateTime FechaInicioTarea { get; set; }

        public DateTime FechaFinTarea {get; set; }

        public string DescripcionTarea { get; set; }

        public string UsuarioIdentityId { get; set; }

        public virtual UsuarioIdentity UsuarioIdentity { get; set; }

        public int OperacionId { get; set; }
        public virtual Operacion Operacion { get; set; }

        public byte EstadoTarea { get; set; }

        public int SolicitudId { get; set; }

        public virtual Solicitud Solicitud { get; set; }

    }
}
