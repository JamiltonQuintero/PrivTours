using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class ServiciosConPermisosViewModel
    {

        public IEnumerable<Servicio> Servicios { set; get; }
        public bool Servicios_Permiso { set; get; }
        public bool Servicios_crear_Permiso { set; get; }
        public bool Servicios_editar_Permiso { set; get; }
        public bool Servicios_activar_inactivar_Permiso { set; get; }
        public bool Servicios_eliminar_Permiso { set; get; }

    }
}
