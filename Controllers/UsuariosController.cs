using Microsoft.AspNetCore.Identity;
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
using PrivTours.Filters;
using PrivTours.Models.Abstract;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace PrivTours.Controllers
{
    [NoCache]
    public class UsuariosController : Controller
    {
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly SignInManager<UsuarioIdentity> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IRolBusiness _iRolBusiness;

        public UsuariosController(UserManager<UsuarioIdentity> userManager, SignInManager<UsuarioIdentity> signInManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IRolBusiness rolBusiness)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _iRolBusiness = rolBusiness;
        }

        [Authorize()]
        public async Task<IActionResult> Index()
        {

            var usuariosPermiso = false;
            var usuariosCrear = false;
            var usuariosEditar = false;
            var usuariosActivarInactivar = false;
            var usuariosEliminar = false;

            var usuarioLogeado = await ObtenerUsuarioLogeado();
            var role = _roleManager.Roles.Where(r => usuarioLogeado.RolSeleccionado.Contains(r.Name)).ToList();
            var permisos = await _iRolBusiness.ObtenerPermisosPorRolId(role[0].Id);

            foreach (var p in permisos)
            {
                var permiso = await _iRolBusiness.ObtenerPermisoPorId(p.PermisoId);


                if (permiso.Nombre == "Usuarios")
                {
                    usuariosPermiso = true;
                }
                else if (permiso.Nombre == "Usuarios-crear")
                {
                    usuariosCrear = true;
                }
                else if (permiso.Nombre == "Usuarios-editar")
                {
                    usuariosEditar = true;
                }
                else if (permiso.Nombre == "Usuarios-activar/inactivar")
                {
                    usuariosActivarInactivar = true;
                }
                else if (permiso.Nombre == "Usuarios-eliminar")
                {
                    usuariosEliminar = true;
                }

            }

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

            var usuariosvm = new UsuariosConPermisosViewModel
            {
                Usuarios = listaUsuariosViewModel,
                Usuarios_Permiso = usuariosPermiso,
                Usuarios_crear_Permiso = usuariosCrear,
                Usuarios_editar_Permiso = usuariosEditar,
                Usuarios_activar_inactivar_Permiso = usuariosActivarInactivar,
                Usuarios_eliminar_Permiso = usuariosEliminar
            };
            return View(usuariosvm);

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

        [Authorize()]
        private async Task<List<string>> ObtenerRolUsuario(UsuarioIdentity usuario)
        {
            return new List<string>(await _userManager.GetRolesAsync(usuario));
        }

        [Authorize()]
        [HttpGet]
        public async Task<IActionResult> Crearusuario()
        {
            ViewData["Roles"] = new SelectList(await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync(), "Name", "Name");
            return View();
        }

        [Authorize()]
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
                            
                            if (error.Code.Equals("DuplicateEmail"))
                            {
                                ModelState.AddModelError("Email", errorMessage);
                            } else
                            {
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

        [Authorize()]
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
        [Authorize()]
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

                            if (error.Code.Equals("DuplicateEmail"))
                            {
                                ModelState.AddModelError("Email", errorMessage);
                            }
                            else
                            {
                                ModelState.AddModelError("", errorMessage);
                            }
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
        [Authorize()]
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

        [Authorize()]
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {

            if (ModelState.IsValid)
            {

                var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RecordarMe, false);

                if (result.Succeeded)
                {
                    //HttpContext.Session.SetString("_Configuration", "Configuration");

                    return RedirectToAction("Dashboard", "Admin");

                }
                ModelState.AddModelError("", "Credenciales incorrectas");
            }

            return View();
        }

        [NoCache]
        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Login", "Usuarios");
            }
            
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RecuperarContrasena()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RecuperarContrasenaAsync(RecuperarContrasenaViewModel recuperarContrasenaViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(recuperarContrasenaViewModel.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    ModelState.AddModelError("", "No existe usuario registrado con ese correo");
                    return View();
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
               
                var callbackUrl = Url.Action(
                    "ResetearContrasena",
                     "Usuarios",
                     new { userId = user.Id, code = code },
                     protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    recuperarContrasenaViewModel.Email,
                    "Restablecer contraseña",
                    $"Hola {user.Nombre}, <br><br> ¿Olvidaste tu contraseña? <br> Hemos recibido una solicitud para restablecer tu contraseña. <br><br> Para restablecer la contraseña, haga clic <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aquí</a>");
     

                TempData["Accion"] = "RecuperarContrasena";
                TempData["Mensaje"] = "Por favor revise su correo, se ha enviado enviado mensaje para recuperación";
                return RedirectToAction("RecuperarContrasenaConfirmacion", "Usuarios");
            }

            return View();
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }
        public IActionResult RecuperarContrasenaConfirmacion()
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
                ModelState.AddModelError("", "No existe usuario registrado con ese correo");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetearContrasenaConfirmacion", "Usuarios");
            }

            foreach (var error in result.Errors)
            {
                if (error.Description.Contains("Invalid token."))
                {
                    error.Description = error.Description.Replace("Invalid token.", "El token es inválido o enlace para restablecer contraseña fue usado");
                }
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        public IActionResult ResetearContrasenaConfirmacion()
        {
            return View();
        }
    }

}
