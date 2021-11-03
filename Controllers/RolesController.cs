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

namespace PrivTours.Controllers
{
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

        public async Task<IActionResult> Index()
        {

            var roles = await _roleManager.Roles.ToListAsync();
           
            var listarolesViewModel = new List<RolViewModel>();

            foreach (var rol in roles)
            {
                var PermisosDetalle = await _iRolBusiness.ObtenerPermisosPorRolId(rol.Id);

                var textoPermisos = "";
                var texto = "";
                var count = 0;
                foreach(var i in PermisosDetalle)
                {
                    count++;
                    if (i.PermisoId == 1)
                    {
                        texto = "MenuEmpleados";
                    }
                    else if (i.PermisoId == 2)
                    {
                        texto = "Listar";
                    }
                    else if (i.PermisoId == 3)
                    {
                        texto = "Crear";
                    }
                    else if (i.PermisoId == 4)
                    {
                        texto = "Editar";
                    }
                    else if (i.PermisoId == 5)
                    {
                        texto = "Actualizar";
                    }
                    else if (i.PermisoId == 6)
                    {
                        texto = "Eliminar";
                    }
                    if (PermisosDetalle.Count == count)
                    {
                        textoPermisos += texto + ". ";
                    } else
                    {
                        textoPermisos += texto + ", ";
                    }
                    
                }
                if(textoPermisos == "")
                {
                    textoPermisos = "Permisos no configurados";
                }
                var rolViewModel = new RolViewModel()
                {
                    Id = rol.Id,
                    NombreRol = rol.Name,
                    DetallePermiso = PermisosDetalle,
                    TextoPermisos = textoPermisos 
                };

                listarolesViewModel.Add(rolViewModel);
            }

            return View(listarolesViewModel);
        }


        public IActionResult CrearRol()
        {
            return View();
        }


        [HttpPost]
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

        public IActionResult ListarRoles()
        {
            return View(_roleManager.Roles);
        }


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
        public async Task<IActionResult> AsignarPermisos(string id)
        {
            ViewData["RoleIdentity"] = id;
            ViewData["Permisos"] = new SelectList(await _iRolBusiness.ObtenerListaPermisos(), "PermisoId", "Nombre");
            return View();
        }

        [HttpPost]
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
