using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class EmpleadosConPermisosViewModel
    {

        public List<UsuarioViewModel> Empleados { set; get; }
        public UsuarioViewModel Empleado { set; get; }
        public bool Empleados_Permiso { set; get; } = false;
        public bool Empleados_crear_Permiso { set; get; } = false;
        public bool Empleados_editar_Permiso { set; get; } = false;
        public bool Empleados_activar_inactivar_Permiso { set; get; } = false;
        public bool Empleados_eliminar_Permiso { set; get; } = false;

    }
}
