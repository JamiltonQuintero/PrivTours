using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using PrivTours.Models.Entities;
using PrivTours.ViewModels;

namespace PrivTours.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly SignInManager<UsuarioIdentity> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public EmpleadosController(UserManager<UsuarioIdentity> userManager, SignInManager<UsuarioIdentity> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
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
            return View(listaUsuariosViewModel);
        }
        private async Task<List<string>> ObtenerRolUsuario(UsuarioIdentity usuario)
        {
            return new List<string>(await _userManager.GetRolesAsync(usuario));
        }

        //[Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Rol"] = "Empleado";
            return View();
        }

        // [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Create(UsuarioViewModel usuarioViewModel)
        {
            if (ModelState.IsValid)
            {
                UsuarioIdentity usuario = new UsuarioIdentity
                {

                    UserName = usuarioViewModel.Email,
                    Email = usuarioViewModel.Email,
                    Nombre = usuarioViewModel.Nombre,
                    Apellido = usuarioViewModel.Apellido,
                    Documento = usuarioViewModel.Documento,
                    Telefono = usuarioViewModel.Telefono,
                    TipoContrato = usuarioViewModel.TipoContrato,
                    EmailConfirmed = true
                };

                try
                {
                    var result = await _userManager.CreateAsync(usuario, usuarioViewModel.Password);

                    if (result.Succeeded)
                    {
                        usuario.Id = await _userManager.GetUserIdAsync(usuario);
                        await _userManager.AddToRoleAsync(usuario, usuarioViewModel.RolSeleccionado);
                        TempData["Accion"] = "Crear";
                        TempData["Mensaje"] = "Se ha creado correctamente el empleado " + usuario.Nombre;
                        return RedirectToAction("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception)
                {
                    ViewData["Rol"] = "Empleado";
                    return View(usuarioViewModel);
                }

            }
            ViewData["Rol"] = "Empleado";
            return View(usuarioViewModel);
        }

        // [Authorize(Roles = "Administrador")]
        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var RolesUsuario = await ObtenerRolUsuario(usuario);
            var usuarioViewModel = new UsuarioViewModel()
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Documento = usuario.Documento,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                Password = usuario.PasswordHash,
                TipoContrato = usuario.TipoContrato,
                ConfirmarPassword = usuario.PasswordHash,
                RolSeleccionado = RolesUsuario.Count == 0 ? "" : RolesUsuario.First()
            };
            ViewData["Rol"] = "Empleado";
            return View(usuarioViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Editar(UsuarioViewModel usuarioViewModel)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmarPassword");
            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = await _userManager.FindByIdAsync(usuarioViewModel.Id);

                    usuario.Email = usuarioViewModel.Email;
                    usuario.Nombre = usuarioViewModel.Nombre;
                    usuario.Apellido = usuarioViewModel.Apellido;
                    usuario.Documento = usuarioViewModel.Documento;
                    usuario.Telefono = usuarioViewModel.Telefono;
                    usuario.TipoContrato = usuarioViewModel.TipoContrato;

                    var RolesUsuario = await ObtenerRolUsuario(usuario);
                    if (!RolesUsuario.Any())
                    {
                        await _userManager.AddToRoleAsync(usuario, usuarioViewModel.RolSeleccionado);
                    }
                    else if (!RolesUsuario.Contains(usuarioViewModel.RolSeleccionado))
                    {
                        await _userManager.RemoveFromRoleAsync(usuario, RolesUsuario.First());
                        await _userManager.AddToRoleAsync(usuario, usuarioViewModel.RolSeleccionado);
                    }

                    await _userManager.UpdateAsync(usuario);
                    TempData["Accion"] = "Editar";
                    TempData["Mensaje"] = "Se ha editado correctamente el empleado " + usuario.Nombre;
                    return RedirectToAction("Index");

                }
                catch (Exception e)
                {
                    TempData["Accion"] = "EditarError";
                    TempData["Mensaje"] = "Lo sentimos, no fue posible editar el empleado" + usuarioViewModel.Nombre;
                    return RedirectToAction("Index");
                }
            }
            return View(usuarioViewModel);
        }
        // GET: Clientes/Delete/5
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return Json(new { data = "error", message = "Id no encontrado" });
            }
            try
            {
                var usuario = await _userManager.FindByIdAsync(id);
                if (usuario == null)
                    return Json(new { data = "error", message = "Empleado a eliminar no existe" });

                await _userManager.DeleteAsync(usuario);

                return Json(new { data = "ok", message = "Empleado " + usuario.Nombre + " fue eliminado correctamente" });
            }
            catch (Exception)
            {
                return Json(new { data = "error", message = "Ocurrió un error al eliminar el empleado" });
            }
        }

        // [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarEstado(string id)
        {
            if (id == null)
            {
                return Json(new { data = "error", message = "Id no encontrado" });
            }
            try
            {
                var usuario = await _userManager.FindByIdAsync(id);
                if (usuario == null)
                    return Json(new { data = "error", message = "Empleado a cambiar estado no existe" });

                if (usuario.LockoutEnd == null)
                {
                    usuario.LockoutEnd = new DateTimeOffset(3000, 12, 31, 23, 59, 59, 0, new TimeSpan(-5, 0, 0));
                }
                else
                {
                    usuario.LockoutEnd = null;
                }
                await _userManager.SetLockoutEndDateAsync(usuario, usuario.LockoutEnd);
                var NuevoEstado = usuario.LockoutEnd == null ? "Activado" : "Inactivado";
                return Json(new { data = "ok", message = "Empleado " + usuario.Nombre + " fue " + NuevoEstado + " correctamente" });
            }
            catch (Exception)
            {
                return Json(new { data = "error", message = "Ocurrió un error al cambiar estado al empleado" });
            }
        }

    }
}
