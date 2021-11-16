﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Entities;
using PrivTours.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Web;

namespace PrivTours.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly SignInManager<UsuarioIdentity> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public UsuariosController(UserManager<UsuarioIdentity> userManager, SignInManager<UsuarioIdentity> signInManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        
        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users.ToListAsync();
            var listaUsuariosViewModel = new List<UsuarioViewModel>();

            foreach (var usuario in usuarios)
            {
                var usuarioViewModel = new UsuarioViewModel()
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Documento = usuario.Documento,
                    Telefono = usuario.Telefono,
                    Email = usuario.Email,
                    Rol = await ObtenerRolUsuario(usuario),
                    Estado = usuario.LockoutEnd == null
                };

                listaUsuariosViewModel.Add(usuarioViewModel);
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
            ViewData["Roles"] = new SelectList(await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync(), "Name", "Name");
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
                        if (!error.Code.Equals("DuplicateUserName"))
                        {
                            string errorMessage = error.Description;
                            if (error.Description.EndsWith("is already taken."))
                            {
                                errorMessage = error.Description.Replace("is already taken", "ya está siendo usado por otro usuario");
                            }
                            ModelState.AddModelError("", errorMessage);
                        }

                    }
                }
                catch (Exception e)
                {
                    ViewData["Roles"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
                    
                    string error = e.Message;
                    if (e.InnerException.Message.Contains("DocumentouniqueIndex"))
                    {
                        error = "Ya existe un usuario con ese Documento";
                        ModelState.AddModelError("Documento", error);
                    }
                    else
                    {
                        ModelState.AddModelError("", error);
                    }
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
                RolSeleccionado = RolesUsuario.Count==0?"":RolesUsuario.First()
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
            try
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmarPassword");
                if (id != usuarioViewModel.Id)
                {
                    ModelState.AddModelError("", "Usuario a editar no corresponde");
                    throw new Exception("ERROR: Usuario a editar no corresponde");
                }

                if (ModelState.IsValid)
                {

                    var usuario = await _userManager.FindByIdAsync(id);

                    usuario.Email = usuarioViewModel.Email;
                    usuario.UserName = usuarioViewModel.Email;
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

                    var result = await _userManager.UpdateAsync(usuario);
                    if (result.Succeeded)
                    {
                        TempData["Accion"] = "Editar";
                        TempData["Mensaje"] = "Se ha actualizado correctamente el usuario " + usuario.Nombre;
                        return RedirectToAction("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        if (!error.Code.Equals("DuplicateUserName"))
                        {
                            string errorMessage = error.Description;
                            if (error.Description.EndsWith("is already taken."))
                            {
                                errorMessage = error.Description.Replace("is already taken", "ya está siendo usado por otro usuario");
                            }
                            ModelState.AddModelError("", errorMessage);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                ViewData["Roles"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
                string error = e.Message;
                if (e.InnerException.Message.Contains("DocumentouniqueIndex"))
                {
                    error = "Ya existe un usuario con ese Documento";
                    ModelState.AddModelError("Documento", error);
                } else
                {
                    ModelState.AddModelError("", error);
                }
                return View(usuarioViewModel);
            }
            ViewData["Roles"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
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


        public IActionResult Login()
        {
            return View();
        }

       // [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {

            if (ModelState.IsValid)
            {

                var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RecordarMe, false);

                if (result.Succeeded)
                {

                    return RedirectToAction("Index", "Usuarios");

                }
                ModelState.AddModelError("", "Credenciales incorrectas");
            }

            return View();
        }

       // [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Usuarios");
        }

       // [AllowAnonymous]
        [HttpGet]
        public IActionResult RecuperarContrasena()
        {
            return View();
        }

       // [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RecuperarContrasenaAsync(RecuperarContrasenaViewModel recuperarContrasenaViewModel)
        {

            /*if (ModelState.IsValid)
            {*/

            /*var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RecordarMe, false);

            if (result.Succeeded)
            {

                return RedirectToAction("Index", "Home");

            }*/
            /* ModelState.AddModelError("", "Error recuperar contraseña");
         }*/
            if (ModelState.IsValid)
            {
                //var user = await _userManager.FindByEmailAsync(Input.Email);
                var user = await _userManager.FindByEmailAsync(recuperarContrasenaViewModel.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                /*var callbackUrl = Url.Page(
                    "/Usuarios/ResetearContrasena",
                    pageHandler: null,
                    values: new { code },
                    protocol: null);*/
                var callbackUrl = Url.Action(
                    "ResetearContrasena",
                     "Usuarios",
                     new { userId = user.Id, code = code },
                     protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    recuperarContrasenaViewModel.Email,
                    "Restablecer contraseña",
                    $"Hola {user.Nombre}, <br><br> ¿Olvidaste tu contraseña? <br> Hemos recibido una solicitud para restablecer tu contraseña. <br><br> Para restablecer la contraseña, haga clic <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aquí</a>");
                    //$"Para restablecer la contraseña, haga clic <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Aquí</a>.");

                TempData["Accion"] = "RecuperarContrasena";
                TempData["Mensaje"] = "Por favor revise su correo, se ha enviado enviado mensaje para recuperación";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetearContrasena(string code)
        {
            if (code == null) 
            {
                return View("Error");
            } else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetearContrasena(string code, ResetearContrasenaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }
    }

}
