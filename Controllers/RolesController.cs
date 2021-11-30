using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrivTours.ViewModels;
using PrivTours.Models.Entities;
using PrivTours.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PrivTours.Controllers
{
    [NoCache]
    public class RolesController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRolBusiness _iRolBusiness;
        private readonly UserManager<UsuarioIdentity> _userManager;

        public RolesController(UserManager<UsuarioIdentity> userManager, RoleManager<IdentityRole> roleManager, IRolBusiness rolBusiness)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _iRolBusiness = rolBusiness;
        }
        [Authorize()]
        public async Task<IActionResult> Index()
        {


            var configuracionPermiso = false;
            var configuracionCrearRol = false;
            var configuracionEditarRol = false;
            var configuracionEliminarRol = false;
            var configuracionEditarPermiso = false;
            var configuracionEliminarPermiso = false;
            var configuracionAsignarPermiso = false;

            var usuarioLogeado = await ObtenerUsuarioLogeado();
            var rol = _roleManager.Roles.Where(r => usuarioLogeado.RolSeleccionado.Contains(r.Name)).ToList();
            var permisos = await _iRolBusiness.ObtenerPermisosPorRolId(rol[0].Id);

            foreach (var p in permisos)
            {
                var permiso = await _iRolBusiness.ObtenerPermisoPorId(p.PermisoId);


                if (permiso.Nombre == "Configuracion")
                {
                    configuracionPermiso = true;
                }
                else if (permiso.Nombre == "Configuracion-crear rol")
                {
                    configuracionCrearRol = true;
                }
                else if (permiso.Nombre == "Configuracion-editar rol")
                {
                    configuracionEditarRol = true;
                }
                else if (permiso.Nombre == "Configuracion-eliminar rol")
                {
                    configuracionEliminarRol = true;
                }
                else if (permiso.Nombre == "Configuracion-asignar permisos")
                {
                    configuracionAsignarPermiso = true;
                }
                else if (permiso.Nombre == "Configuracion-eliminar permisos")
                {
                    configuracionEliminarPermiso = true;
                }
                else if (permiso.Nombre == "Configuracion-editar permisos")
                {
                    configuracionEditarPermiso = true;
                }

            }

            var roles = await _roleManager.Roles.ToListAsync();
           
            var listarolesViewModel = new List<RolViewModel>();

            foreach (var role in roles)
            {
                var PermisosDetalle = await _iRolBusiness.ObtenerPermisosPorRolId(role.Id);

                var rolViewModel = new RolViewModel()
                {
                    Id = role.Id,
                    NombreRol = role.Name,
                    DetallePermiso = PermisosDetalle
                };

                listarolesViewModel.Add(rolViewModel);
            }


            var configuracionvm = new ConfiguracionConPermisosViewModel
            {
                Roles = listarolesViewModel,
                Configuracion_Permiso = configuracionPermiso,
                Configuracion_crear_rol_Permiso = configuracionCrearRol,
                Configuracion_editar_rol_Permiso = configuracionEditarRol,
                Configuracion_eliminar_rol_Permiso = configuracionEliminarRol,
                Configuracion_editar_permi_Permiso = configuracionEditarPermiso,
                Configuracion_eliminar_permi_Permiso = configuracionEliminarPermiso,
                Configuracion_asignar_permi_Permiso = configuracionAsignarPermiso
            };
            return View(configuracionvm);
        }

        [Authorize()]
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
        [Authorize()]
        private async Task<List<string>> ObtenerRolUsuario(UsuarioIdentity usuario)
        {
            return new List<string>(await _userManager.GetRolesAsync(usuario));
        }

        [Authorize()]
        public IActionResult CrearRol()
        {
            return View();
        }


        [HttpPost]
        [Authorize()]
        public async Task<IActionResult> CrearRol(RolViewModel rolViewModel)
        {

            if (ModelState.IsValid)
            {

                IdentityRole rol = new IdentityRole
                {
                    Name = rolViewModel.NombreRol
                };

                var result = await _roleManager.CreateAsync(rol);

                if (result.Succeeded)
                {
                    TempData["Accion"] = "CrearRol";
                    TempData["Mensaje"] = "Rol " + rol.Name + " creado";
                    return RedirectToAction("Index", "Roles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }


            }

            return View(rolViewModel);
        }
        [Authorize()]
        public IActionResult ListarRoles()
        {
            return View(_roleManager.Roles);
        }

        [Authorize()]
        public async Task<IActionResult> EliminarRol(string roleIdentityId)
        {

            try
            {
                var rol = await _roleManager.FindByIdAsync(roleIdentityId);
                if (rol == null)
                {
                    ViewData["Error"] = $"El rol con id {roleIdentityId} no se encontró";
                    return View("NotFound");
                }
                await _roleManager.DeleteAsync(rol);
                return Json(new { status = true });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }

        [Authorize()]
        public async Task<IActionResult> EditarRol(string id)
        {
            var rol = await _roleManager.FindByIdAsync(id);
            if (rol == null)
            {
                ViewData["Error"] = $"El rol con id {id} no se encontró";
                return View("NotFound");
            }
            var editarRolViewModel = new EditarRolViewModel
            {
                Id = rol.Id,
                NombreRol = rol.Name
            };

            return View(editarRolViewModel);
        }

        
        [HttpPost]
        [Authorize()]
        public async Task<IActionResult> EditarRol(EditarRolViewModel editarRolViewModel)
        {
            var rol = await _roleManager.FindByIdAsync(editarRolViewModel.Id);
            if (rol == null)
            {
                ViewData["Error"] = $"El rol con id {editarRolViewModel.Id} no se encontró";
                return View("NotFound");
            }
            else
            {
                rol.Name = editarRolViewModel.NombreRol;
                var result = await _roleManager.UpdateAsync(rol);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);

                }
            }


            return View(editarRolViewModel);
        }
        
        [HttpGet]
        [Authorize()]
        public async Task<IActionResult> AsignarPermisos(string id)
        {
            ViewData["RoleIdentity"] = id;
            ViewData["Permisos"] = new SelectList(await _iRolBusiness.ObtenerListaPermisos(), "PermisoId", "Nombre");
            return View();
        }

        [HttpPost]
        [Authorize()]
        public async Task<IActionResult> AsignarPermisos(string roleIdentityId, string[] permisos)
        {
            try
            {
                RolViewModel rolViewModel = new RolViewModel
                {
                    RoleIdentityId = roleIdentityId,
                    Permisos = permisos
                };
                await _iRolBusiness.GuardarDetallePermisos(rolViewModel);
                return Json(new { status = true });
            }
            catch(Exception e)
            {
                return Json(new { status = false });
            }
        }

        [HttpGet]
        [Authorize()]
        public async Task<IActionResult> EditarPermisos(string id)
        {

                var permisos = await _iRolBusiness.ObtenerListaPermisos();

                var PermisosDetalle = await _iRolBusiness.ObtenerPermisosPorRolId(id);
                var PermisosSeleccionados = new List<Permiso>();
                foreach (DetallePermiso element in PermisosDetalle)
                {
                    var permiso = permisos.Find(ps => ps.PermisoId == element.PermisoId);
                    PermisosSeleccionados.Add(permiso);
                }

            EditarPermisoViewModel editarPermisoViewModel = new EditarPermisoViewModel
                {
                    RoleIdentityId = PermisosDetalle[0].RoleIdentityId,
                    Permisos = permisos,
                    PermisosSeleccionados = PermisosSeleccionados
                };
            ViewData["PermisosSeleccionados"] = new SelectList(PermisosSeleccionados, "PermisoId", "Nombre");
            return View(editarPermisoViewModel);
           
        }

        [HttpPost]
        [Authorize()]
        public async Task<IActionResult> EditarPermisos(string roleIdentityId, string[] permisos)
        {
            try
            {
               
                var response = await _iRolBusiness.EliminarPermisosPorRolId(roleIdentityId);
                if (response)
                {
                    RolViewModel rolViewModel = new RolViewModel
                    {
                        RoleIdentityId = roleIdentityId,
                        Permisos = permisos
                    };
                    await _iRolBusiness.GuardarDetallePermisos(rolViewModel);
                    return Json(new { status = true });
                } else
                {
                    return Json(new { status = false });
                }

            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }
        }

        [HttpGet]
        [Authorize()]
        public async Task<IActionResult> EliminarPermisos(string roleIdentityId)
        {
            try
            {

                var response = await _iRolBusiness.EliminarPermisosPorRolId(roleIdentityId);
                if (response)
                {
                    return Json(new { status = true });
                } else
                {
                    return Json(new { status = false });
                }

            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }
        }

        [HttpGet]
        [Authorize()]
        public async Task<IActionResult> ObtenerPermisosPorRolId(string roleIdentityId)
        {
            try
            {

                var PermisosDetalle = await _iRolBusiness.ObtenerPermisosPorRolId(roleIdentityId);
                if (PermisosDetalle.Count > 0 )
                {
                    return Json(new { status = true, data = PermisosDetalle });
                }
                else
                {
                    return Json(new { status = false });
                }

            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }
        }

    }

}
