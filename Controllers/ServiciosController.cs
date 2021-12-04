using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrivTours.Filters;
using PrivTours.Models.Abstract;
using PrivTours.Models.DAL;
using PrivTours.Models.Entities;
using PrivTours.ViewModels;

namespace PrivTours.Controllers
{
    [NoCache]
    public class ServiciosController : Controller
    {
        private readonly IServiciosBusiness _serviciosBusiness;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRolBusiness _iRolBusiness;

        public ServiciosController(UserManager<UsuarioIdentity> userManager, IServiciosBusiness serviciosBusiness, RoleManager<IdentityRole> roleManager, IRolBusiness rolBusiness)
        {
            _serviciosBusiness = serviciosBusiness;
            _userManager = userManager;
            _roleManager = roleManager;
            _iRolBusiness = rolBusiness;
        }

        // GET: Servicios
        [Authorize()]
        public async Task<IActionResult> Index()
        {
        
            var serviciovm =await ObtenerPermisosUsuarioLogeado();
            if (serviciovm.Servicios_Permiso)
            {
               serviciovm.Servicios = await _serviciosBusiness.ObtenerListaServicios(); ;
            }
            
            return View(serviciovm);
        }

        [Authorize()]
        private async Task<ServiciosConPermisosViewModel> ObtenerPermisosUsuarioLogeado()
        {

            var servicioPermiso = new ServiciosConPermisosViewModel();
            var usuarioLogeado = await ObtenerUsuarioLogeado();
            var roles = await _roleManager.Roles.ToListAsync();
            var rolEmpleado = roles.Find(r => r.Name == usuarioLogeado.RolSeleccionado);
            var permisos = await _iRolBusiness.ObtenerPermisosPorRolId(rolEmpleado.Id);

            foreach (var p in permisos)
            {
                var permiso = await _iRolBusiness.ObtenerPermisoPorId(p.PermisoId);


                if (permiso.Nombre == "Servicios")
                {
                    servicioPermiso.Servicios_Permiso = true;
                }
                else if (permiso.Nombre == "Servicios-crear")
                {
                    servicioPermiso.Servicios_crear_Permiso = true;
                }
                else if (permiso.Nombre == "Servicios-editar")
                {
                    servicioPermiso.Servicios_editar_Permiso = true;
                }
                else if (permiso.Nombre == "Servicios-activar/inactivar")
                {
                    servicioPermiso.Servicios_activar_inactivar_Permiso = true;
                }
                else if (permiso.Nombre == "Servicios-eliminar")
                {
                    servicioPermiso.Servicios_eliminar_Permiso = true;
                }

            }
            return servicioPermiso;
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

        // GET: Servicios/Details/5
        [Authorize()]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _serviciosBusiness.ObtenerServicioPorId(id.Value);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        // GET: Servicios/Create
        [Authorize()]
        public async Task<IActionResult> Create()
        {
            var serviciovm = await ObtenerPermisosUsuarioLogeado();
            return View(serviciovm);
        }

        // POST: Servicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize()]
        public async Task<IActionResult> Create(ServiciosConPermisosViewModel serviciosConPermisosViewModel)
        {
            var servicio = serviciosConPermisosViewModel.Servicio;
  
                try
                {

                    servicio.Estado = true;
                    await _serviciosBusiness.GuardarServicio(servicio);
                    TempData["Accion"] = "Crear";
                    TempData["Mensaje"] = "Se ha creado correctamente el servicio " + servicio.Nombre;
                    return RedirectToAction("Index");


                }
                catch (Exception)
                {
                    return Json(new { data = "error" });
                }

        }

        // GET: Servicios/Edit/5
        [Authorize()]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var serviciovm = await ObtenerPermisosUsuarioLogeado();
            if (serviciovm.Servicios_editar_Permiso)
            {
                var servicio = await _serviciosBusiness.ObtenerServicioPorId(id.Value);
                if (servicio == null)
                {
                    return NotFound();
                } else
                {
                    serviciovm.Servicio = servicio;
                }
                
            }
   
            return View(serviciovm);
        }

        // POST: Servicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize()]
        public async Task<IActionResult> Edit(int id, ServiciosConPermisosViewModel serviciosConPermisosViewModel)
        {
            var servicio = serviciosConPermisosViewModel.Servicio;
                try
                {

                    await _serviciosBusiness.EditarServicio(servicio);
                    TempData["Accion"] = "Editar";
                    TempData["Mensaje"] = "Se ha editado correctamente el servicioeado " + servicio.Nombre;
                    return RedirectToAction("Index");
                    
                }
                catch (Exception)
                {

                    return Json(new { data = "error" });
                }
           
        }

        // GET: Servicios/Delete/5
        [Authorize()]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { data = "error", message = "Id no encontrado" });
            }
            try
            {
                var servicio = await _serviciosBusiness.ObtenerServicioPorId(id.Value);
                if (servicio == null)

                    return Json(new { data = "error", message = "Servicio a eliminar no existe" });
                await _serviciosBusiness.EliminarServicio(servicio);
                return Json(new { data = "ok", message = "Servicio " + servicio.Nombre + " fue eliminado correctamente" });
            }
            catch (Exception)
            {
                return Json(new { data = "error", message = "Ocurrió un error al eliminar al servicio" });
            }
        }


        [HttpGet]
        [Authorize()]
        public async Task<IActionResult> CambiarEstado(int? id)
        {
            if (id == null)
            {
                return Json(new { data = "error", message = "Id no encontrado" });
            }
            try
            {
                var servicio = await _serviciosBusiness.ObtenerServicioPorId(id.Value);
                if (servicio == null)
                    return Json(new { data = "error", message = "Empleado a cambiar estado no existe" });
                if (servicio.Estado)
                {
                    servicio.Estado = false;
                }
                else
                {
                    servicio.Estado = true;
                }
                await _serviciosBusiness.EditarServicio(servicio);

                var NuevoEstado = servicio.Estado == true ? "Activado" : "Inactivado";
                return Json(new { data = "ok", message = "Servicio " + servicio.Nombre + " fue " + NuevoEstado + " correctamente" });
            }
            catch (Exception)
            {
                return Json(new { data = "error", message = "Ocurrió un error al cambiar estado al servicio" });
            }
        }

    }
}

