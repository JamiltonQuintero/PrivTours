
using System.ComponentModel.DataAnnotations;


namespace PrivTours.Models.Entities
{
    public class DetallePermiso
    {
        [Key]
        public int DetallePermisoId { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public int PermisoId { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string RoleIdentityId { get; set; }
         public Permiso Permiso { get; set; }
        public RoleIdentity RoleIdentity { get; set; }
    }
}
