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
using Dapper;
namespace PrivTours.Controllers
{
    [NoCache]
    public class ReportesController : Controller
    {
        private readonly IClientesBusiness _clientesBusiness;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRolBusiness _iRolBusiness;

        public ReportesController(IClientesBusiness clientesBusiness, RoleManager<IdentityRole> roleManager, IRolBusiness rolBusiness, UserManager<UsuarioIdentity> userManager)
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
        

    }
}

