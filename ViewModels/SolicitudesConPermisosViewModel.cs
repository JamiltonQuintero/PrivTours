using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class SolicitudesConPermisosViewModel
    {

        public  SolicitudViewModel Solicitud { set; get; }

        public bool SolicitudesDeServicio_Permiso { set; get; }
        public bool SolicitudesDeServicio_crear_Permiso { set; get; }
        public bool SolicitudesDeServicio_editar_Permiso { set; get; }
        public bool SolicitudesDeServicio_filtrar_Permiso { set; get; }
        public bool SolicitudesDeServicio_eliminar_tarea_Permiso { set; get; }
        public bool SolicitudesDeServicio_editar_tareaPermiso { set; get; }
        public bool SolicitudesDeServicio_crear_tareaPermiso { set; get; }

    }
}
