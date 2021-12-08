using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrivTours.Filters;
using PrivTours.Models.Abstract;
using PrivTours.Models.Entities;
using PrivTours.ViewModels;

namespace PrivTours.Controllers
{
    [NoCache]
    public class ClientesController : Controller
    {
        private readonly IClientesBusiness _clientesBusiness;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRolBusiness _iRolBusiness;

        public ClientesController(IClientesBusiness clientesBusiness, RoleManager<IdentityRole> roleManager, IRolBusiness rolBusiness, UserManager<UsuarioIdentity> userManager)
        {
            _clientesBusiness = clientesBusiness;
            _userManager = userManager;
            _roleManager = roleManager;
            _iRolBusiness = rolBusiness;
        }

        // GET: Clientes
        [Authorize()]
        public async Task<IActionResult> Index()
        {
 
            var clientevm = await ObtenerPermisosUsuarioLogeado();
            if (clientevm.Clientes_Permiso)
            {
                clientevm.Clientes = await _clientesBusiness.ObtenerListaClientes();
            }

            return View(clientevm);
        }



        [Authorize()]
        private async Task<ClientesConPermisosViewModel> ObtenerPermisosUsuarioLogeado()
        {
            var clientePermiso = new ClientesConPermisosViewModel();
            var usuarioLogeado = await ObtenerUsuarioLogeado();
            var roles = await _roleManager.Roles.ToListAsync();
            var rolEmpleado = roles.Find(r => r.Name == usuarioLogeado.RolSeleccionado);
            var permisos = await _iRolBusiness.ObtenerPermisosPorRolId(rolEmpleado.Id);

            foreach (var p in permisos)
            {
                var permiso = await _iRolBusiness.ObtenerPermisoPorId(p.PermisoId);

                if (permiso.Nombre == "Clientes")
                {
                    clientePermiso.Clientes_Permiso = true;
                }
                else if (permiso.Nombre == "Clientes-crear")
                {
                    clientePermiso.Clientes_crear_Permiso = true;
                }
                else if (permiso.Nombre == "Clientes-editar")
                {
                    clientePermiso.Clientes_editar_Permiso = true;
                }
                else if (permiso.Nombre == "Clientes-activar/inactivar")
                {
                    clientePermiso.Clientes_activar_inactivar_Permiso = true;
                }
                else if (permiso.Nombre == "Clientes-eliminar")
                {
                    clientePermiso.Clientes_eliminar_Permiso = true;
                }

            }
            return clientePermiso;
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

        // GET: Clientes/Details/5
        [Authorize()]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _clientesBusiness.ObtenerClientePorId(id.Value);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize()]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    cliente.Estado = true;
                    await _clientesBusiness.GuardarCliente(cliente);
                    TempData["Accion"] = "Crear";
                    TempData["Mensaje"] = "El cliente " + cliente.Nombre + " ha sido creado correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return View(cliente);
                }
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        [Authorize()]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientevm = await ObtenerPermisosUsuarioLogeado();
            var servicio = await _clientesBusiness.ObtenerClientePorId(id.Value);
            if (clientevm.Clientes_editar_Permiso)
            {
                
                if (servicio == null)
                {
                    return NotFound();
                }
                else
                {
                    clientevm.Cliente = servicio;
                }

            }

            return View(servicio);

        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize()]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.ClienteId)
            {
                return Json(new { data = "error" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _clientesBusiness.EditarCliente(cliente);
                    TempData["Accion"] = "Editar";
                    TempData["Mensaje"] = "El cliente " + cliente.Nombre+ " ha sido editado correctamente";
                    return RedirectToAction("Index");

                }
                catch (Exception)
                {

                    return Json(new { data = "error" });
                }
            }
            return Json(new { data = "error" });
        }

        // GET: Clientes/Delete/5
        [Authorize()]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { data = "error", message = "Id no encontrado" });
            }
            try
            {
                var cliente = await _clientesBusiness.ObtenerClientePorId(id.Value);
                if (cliente == null)

                    return Json(new { data = "error", message = "Cliente a eliminar no existe" });
                await _clientesBusiness.EliminarCliente(cliente);

                return Json(new { data = "ok", message = "El cliente " + cliente.Nombre + " fue eliminado correctamente" });
            }
            catch (Exception)
            {
                return Json(new { data = "error", message = "Ocurrió un error al eliminar al cliente" });
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
                var cliente = await _clientesBusiness.ObtenerClientePorId(id.Value);
                if (cliente == null)
                    return Json(new { data = "error", message = "El cliente al que intentas cambiar su estado no existe" });
                if (cliente.Estado)
                {
                    cliente.Estado = false;
                }
                else
                {
                    cliente.Estado = true;
                }
                await _clientesBusiness.EditarCliente(cliente);

                var NuevoEstado = cliente.Estado == true ? "Activado" : "Inactivado";
                return Json(new { data = "ok", message = "El cliente " + cliente.Nombre + " fue " + NuevoEstado + " correctamente" });
            }
            catch (Exception)
            {
                return Json(new { data = "error", message = "Ocurrió un error al cambiar estado del cliente" });
            }
        }

    }
}

