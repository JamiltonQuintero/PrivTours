using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.DAL
{
    public class DbContextPriv : IdentityDbContext
    {
        public DbContextPriv(DbContextOptions<DbContextPriv> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Empleado> Empleados { get; set; }

        public DbSet<Servicio> Servicios { get; set; }

        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Operacion> Operaciones { get; set; }
        public DbSet<UsuarioIdentity> UsuariosIdentity { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<RoleIdentity> RolesIdentity { get; set; }
        public DbSet<DetallePermiso> DetallePermisos { get; set; }
    }
}
