using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class UsuariosConPermisosViewModel
    {

        public List<UsuarioViewModel> Usuarios { set; get; }

        public bool Usuarios_Permiso { set; get; }
        public bool Usuarios_crear_Permiso { set; get; }
        public bool Usuarios_editar_Permiso { set; get; }
        public bool Usuarios_activar_inactivar_Permiso { set; get; }
        public bool Usuarios_eliminar_Permiso { set; get; }

    }
}
