using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class ClientesConPermisosViewModel
    {

        public IEnumerable<Cliente> Clientes { set; get; }
        public bool Clientes_Permiso { set; get; }
        public bool Clientes_crear_Permiso { set; get; }
        public bool Clientes_editar_Permiso { set; get; }
        public bool Clientes_activar_inactivar_Permiso { set; get; }
        public bool Clientes_eliminar_Permiso { set; get; }

    }
}
