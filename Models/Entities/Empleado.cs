using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Entities
{
    public class Empleado
    {

        public int EmpleadoId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Celular { get; set; }
        public int TipoContrato { get; set; }

    }
}
