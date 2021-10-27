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
                if ("Empleado".Equals(rol))
                {
                    var usuarioViewModel = new UsuarioViewModel()
                    {
                        Id = usuario.Id,
                        Nombre = usuario.Nombre,
                        Apellido = usuario.Apellido,
                        Documento = usuario.Documento,
                        Telefono = usuario.Telefono,
                        Email = usuario.Email,
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
        public async Task<IActionResult> Crearusuario()
        {
            ViewData["Roles"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        // [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Crearusuario(UsuarioViewModel usuarioViewModel)
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
                        TempData["Mensaje"] = "Se ha creado correctamente el usuario " + usuario.Nombre;
                        return RedirectToAction("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception)
                {
                    ViewData["Roles"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
                    return View(usuarioViewModel);
                }

            }
            ViewData["Roles"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View(usuarioViewModel);
        }

        // [Authorize(Roles = "Administrador")]
        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Editar(string id)
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
                ConfirmarPassword = usuario.PasswordHash,
                RolSeleccionado = RolesUsuario.Count == 0 ? "" : RolesUsuario.First()
            };
            ViewData["Roles"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View(usuarioViewModel);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [Authorize(Roles = "Administrador")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(string id, [Bind("Id,Nombre,Apellido,Documento,Email,Telefono,RolSeleccionado")] UsuarioViewModel usuarioViewModel)
        {

            if (id != usuarioViewModel.Id)
            {
                return Json(new { data = "Error" });
            }

            ModelState.Remove("Password");
            ModelState.Remove("ConfirmarPassword");
            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = await _userManager.FindByIdAsync(id);

                    usuario.Email = usuarioViewModel.Email;
                    usuario.Nombre = usuarioViewModel.Nombre;
                    usuario.Apellido = usuarioViewModel.Apellido;
                    usuario.Documento = usuarioViewModel.Documento;
                    usuario.Telefono = usuarioViewModel.Telefono;

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
                    return Json(new { data = "ok" });

                }
                catch (Exception e)
                {

                    return Json(new { data = e.Message });
                }
            }
            return Json(new { data = "error" });
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
                    return Json(new { data = "error", message = "Usuario a eliminar no existe" });

                await _userManager.DeleteAsync(usuario);

                return Json(new { data = "ok", message = "Usuario " + usuario.Nombre + " fue eliminado correctamente" });
            }
            catch (Exception)
            {
                return Json(new { data = "error", message = "Ocurrió un error al eliminar el usuario" });
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
                    return Json(new { data = "error", message = "Usuario a cambiar estado no existe" });

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
                return Json(new { data = "ok", message = "Usuario " + usuario.Nombre + " fue " + NuevoEstado + " correctamente" });
            }
            catch (Exception)
            {
                return Json(new { data = "error", message = "Ocurrió un error al cambiar estado al usuario" });
            }
        }

    }
}
