using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class TareasConPermisosViewModel
    {

        public  List<SolicitudViewModel> Tareas { set; get; }

        public bool Tareas_Permiso { set; get; }
        public bool Tareas_inicar_tarea_Permiso { set; get; }
        public bool Tareas_cancelar_tarea_Permiso { set; get; }
        public bool Tareas_terminar_tarea_Permiso { set; get; }
        public bool Tareas_filtrar_Permiso { set; get; }
    }
}
