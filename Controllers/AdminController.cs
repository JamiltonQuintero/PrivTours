using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrivTours.Models.Abstract;
using PrivTours.Models.DAL;
using PrivTours.Models.Entities;
using Microsoft.AspNetCore.Identity;
using PrivTours.ViewModels;
using Microsoft.AspNetCore.Authorization;
using PrivTours.Filters;
using PrivTours.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PrivTours.Controllers
{
    [NoCache]
    public class AdminController : Controller
    {

        private readonly IAdminBusiness _iAdminBusiness;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly IRolBusiness _iRolBusiness;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(IAdminBusiness adminBusiness, UserManager<UsuarioIdentity> userManager, IRolBusiness rolBusiness, RoleManager<IdentityRole> roleManager)
        {
            _iAdminBusiness = adminBusiness;
            _userManager = userManager;
            _roleManager = roleManager;
            _iRolBusiness = rolBusiness;
        }

        [Authorize()]
        public async Task<IActionResult> Dashboard()
        {


            var usuariosPermiso = new ReportesConPermisosViewModel();
            var usuarioLogeado = await ObtenerUsuarioLogeado();
            var roles = await _roleManager.Roles.ToListAsync();
            var rolEmpleado = roles.Find(r => r.Name == usuarioLogeado.RolSeleccionado);
            var permisos = await _iRolBusiness.ObtenerPermisosPorRolId(rolEmpleado.Id);

            foreach (var p in permisos)
            {
                var permiso = await _iRolBusiness.ObtenerPermisoPorId(p.PermisoId);


                if (permiso.Nombre == "Generar Reporte")
                {
                    usuariosPermiso.Reporte_Generar_Permiso = true;
                }
                
            }

            var reporte = _iAdminBusiness.ReporteDashboar();
            var usuarios = await _userManager.Users.ToListAsync();
            var listaUsuariosViewModel = new List<UsuarioViewModel>();

            foreach (var usuario in usuarios)
            {
                var rol = await ObtenerRolUsuario(usuario);
                if ("Empleado".Equals(rol[0]))
                {
                    var usuarioViewModel = new UsuarioViewModel()
                    {
                        Id = usuario.Id,
                        Nombre = usuario.Nombre,
                        Apellido = usuario.Apellido,
                        Documento = usuario.Documento,
                        Telefono = usuario.Telefono,
                        Email = usuario.Email,
                        TipoContrato = usuario.TipoContrato,
                        Estado = usuario.LockoutEnd == null
                    };
                    listaUsuariosViewModel.Add(usuarioViewModel);
                }
            }
            reporte.TotEmpleado = listaUsuariosViewModel.Count();
            usuariosPermiso.Reporte = reporte;
            return View(usuariosPermiso);
        }

        private async Task<List<string>> ObtenerRolUsuario(UsuarioIdentity usuario)
        {
            return new List<string>(await _userManager.GetRolesAsync(usuario));
        }

        public ActionResult Reportes()
        {
            return View();
        }


        private async Task<UsuarioViewModel> ObtenerUsuarioLogeado()
        {
            var usuarioViewModel = new UsuarioViewModel();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null && userId != "")
            {
                var usuario = await _userManager.FindByIdAsync(userId);
                var RolesUsuario = await ObtenerRolUsuario(usuario);
                usuarioViewModel.Id = usuario.Id;
                usuarioViewModel.Nombre = usuario.Nombre;
                usuarioViewModel.Apellido = usuario.Apellido;
                usuarioViewModel.Documento = usuario.Documento;
                usuarioViewModel.Email = usuario.Email;
                usuarioViewModel.Telefono = usuario.Telefono;
                usuarioViewModel.Password = usuario.PasswordHash;
                usuarioViewModel.ConfirmarPassword = usuario.PasswordHash;
                usuarioViewModel.RolSeleccionado = RolesUsuario.Count == 0 ? "" : RolesUsuario.First();
            }

            return usuarioViewModel;
        }

    }
}
