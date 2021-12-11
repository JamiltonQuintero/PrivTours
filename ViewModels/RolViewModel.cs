using PrivTours.Models.DAL;
using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class RolViewModel 
    {
        public string Id { get; set; }
    
        [DisplayName("Nombre Rol")]
        [Required(ErrorMessage = "Debes ingresar un rol.")]
        [Compare("NombreRol", ErrorMessage = "El rol que tratas de ingresar ya se encuentra registrado.")]
        public string NombreRol { get; set; }
        public string[] Permisos { get; set; }
        public string[] PermisosSinSelec { get; set; }

        public string RoleIdentityId { get; set; }
        public RoleIdentity RoleIdentity { get; set; }
        public List<DetallePermiso> DetallePermiso { get; set; }

        [Required(ErrorMessage = "Por favor ingresa una descripción.")]
        public string Descripcion { get; set; }

        public string TextoPermisos { get; set; }
    }

   
    }

