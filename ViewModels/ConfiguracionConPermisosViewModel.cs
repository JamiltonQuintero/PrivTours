using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class ConfiguracionConPermisosViewModel
    {

        public List<RolViewModel> Roles { set; get; }

        public bool Configuracion_Permiso { set; get; }
        public bool Configuracion_crear_rol_Permiso { set; get; }
        public bool Configuracion_editar_rol_Permiso { set; get; }
        public bool Configuracion_eliminar_rol_Permiso { set; get; }
        public bool Configuracion_editar_permi_Permiso { set; get; }
        public bool Configuracion_eliminar_permi_Permiso { set; get; }
        public bool Configuracion_asignar_permi_Permiso { set; get; }

    }
}
