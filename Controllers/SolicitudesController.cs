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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrivTours.Controllers
{
    [NoCache]
    public class SolicitudesController : Controller
    {


        private readonly ISolicitudesBusiness _solicitudesBuseness;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly ITareasBusiness _tareasBusiness;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRolBusiness _iRolBusiness;
        public SolicitudesController(UserManager<UsuarioIdentity> userManager, ISolicitudesBusiness solicitudesBuseness, ITareasBusiness tareasBusiness,
            RoleManager<IdentityRole> roleManager, IRolBusiness rolBusiness)
        {
            _solicitudesBuseness = solicitudesBuseness;
            _userManager = userManager;
            _tareasBusiness = tareasBusiness; 
            _roleManager = roleManager;
            _iRolBusiness = rolBusiness;
        }
        [Authorize()]
        public async Task<IActionResult> Index()
        {



            var SolicitudesDeServicio = false;
            var SolicitudesDeServicioEditar = false;
            var SolicitudesDeServicioCrear = false;
            var SolicitudesDeServicioFiltrar = false;
            var SolicitudesDeServicioCrearTarea = false;
            var SolicitudesDeServicioEditarTarea = false;
            var SolicitudesDeServicioEliminarTarea = false;

            var usuarioLogeado = await ObtenerUsuarioLogeado();
            var role = _roleManager.Roles.Where(r => usuarioLogeado.RolSeleccionado.Contains(r.Name)).ToList();
            var permisos = await _iRolBusiness.ObtenerPermisosPorRolId(role[0].Id);

            foreach (var p in permisos)
            {
                var permiso = await _iRolBusiness.ObtenerPermisoPorId(p.PermisoId);


                if (permiso.Nombre == "Solicitudes de servicio")
                {
                    SolicitudesDeServicio = true;
                }
                else if (permiso.Nombre == "Solicitudes de servicio-filtrar")
                {
                    SolicitudesDeServicioEditar = true;
                }
                else if (permiso.Nombre == "Solicitudes de servicio-crear")
                {
                    SolicitudesDeServicioCrear = true;
                }
                else if (permiso.Nombre == "Solicitudes de servicio-editar")
                {
                    SolicitudesDeServicioFiltrar = true;
                }
                else if (permiso.Nombre == "Solicitudes de servicio-crear tarea")
                {
                    SolicitudesDeServicioCrearTarea = true;
                }
                else if (permiso.Nombre == "Solicitudes de servicio-editar tarea")
                {
                    SolicitudesDeServicioEditarTarea = true;
                }
                else if (permiso.Nombre == "Solicitudes de servicio-eliminar tarea")
                {
                    SolicitudesDeServicioEliminarTarea = true;
                }

            }

            var usuarios = await _userManager.Users.ToListAsync();
            var listaUsuarios = new List<UsuarioIdentity>();

            foreach (var usuario in usuarios)
            {
                var rol = await ObtenerRolUsuario(usuario);
                if ("Empleado".Equals(rol[0]))
                {

                    listaUsuarios.Add(usuario);
                }
            }

            ViewData["Clientes"] = new SelectList(await _solicitudesBuseness.ObtenerListaClientes(), "ClienteId", "Nombre");
            ViewData["Empleados"] = new SelectList(listaUsuarios, "Id", "Nombre");
            ViewData["Servicios"] = new SelectList(await _solicitudesBuseness.ObtenerListaServicios(), "ServicioId", "Nombre");
            ViewData["Operaciones"] = new SelectList(await _solicitudesBuseness.ObtenerListaOperaciones(), "OperacionId", "Nombre");


            var solicitudesvm = new SolicitudesConPermisosViewModel
            {
                Solicitud = null,
                SolicitudesDeServicio_Permiso = SolicitudesDeServicio,
                SolicitudesDeServicio_crear_Permiso = SolicitudesDeServicioEditar,
                SolicitudesDeServicio_editar_Permiso = SolicitudesDeServicioCrear,
                SolicitudesDeServicio_filtrar_Permiso = SolicitudesDeServicioFiltrar,
                SolicitudesDeServicio_eliminar_tarea_Permiso = SolicitudesDeServicioCrearTarea,
                SolicitudesDeServicio_editar_tareaPermiso = SolicitudesDeServicioEditarTarea,
                SolicitudesDeServicio_crear_tareaPermiso = SolicitudesDeServicioEliminarTarea
            };
            return View(solicitudesvm);
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
        public async Task<IActionResult> ObtenerListaClientes()

        {
            try
            {
                var clientes = await _solicitudesBuseness.ObtenerListaClientes();
                return Json(new { status = true, data = clientes });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }

        [Authorize()]
        public async Task<IActionResult> ObtenerListaEmpleados()

        {
           try
            {

                var usuarios = await _userManager.Users.ToListAsync();
                var listaUsuarios = new List<UsuarioIdentity>();

                foreach (var usuario in usuarios)
                {
                    var rol = await ObtenerRolUsuario(usuario);
                    if ("Empleado".Equals(rol[0]) && usuario.LockoutEnd == null)
                    {

                        listaUsuarios.Add(usuario);
                    }
                }

                return Json(new { status = true, data = listaUsuarios });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }
        [Authorize()]
        public async Task<IActionResult> ObtenerListaServicios()

        {
            try
            {
                var servicios = await _solicitudesBuseness.ObtenerListaServicios();
                return Json(new { status = true, data = servicios });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }
        [Authorize()]
        public async Task<IActionResult> ObtenerSolicitudesValidadndoDisponibilidad(SolicitudViewModel solicitudViewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false });
            }

            try
            {

                var lIdsSolicitudes = await _solicitudesBuseness.ObtenerSolicitudesPorEmpleadosSeleccionados(solicitudViewModel.Empleados);
                var lSolicitudes = new List<SolicitudViewModel>();


                foreach (int i in lIdsSolicitudes)
                {
                    var soli = await _solicitudesBuseness.ObtenerSolicitudPorId(i);
                    var count = 0;
                    var textonNombres = "";
                    var texto = "";
                        
                    /*foreach (var e in soli.DetalleSolicitudEmpleado)
                    {
                        count++;
                        var usuario = await _userManager.FindByIdAsync(e.UsuarioIdentityId);
                        texto = usuario.Nombre + usuario.Apellido;
                        if (soli.DetalleSolicitudEmpleado.Count == count)
                        {
                           
                            textonNombres += texto + " ";
                        }
                        else
                        {
                            textonNombres += texto + ", ";
                        }
                    }*/
                    
                    SolicitudViewModel solicitud = new SolicitudViewModel
                    {
                        FechaInicio = soli.FechaInicio,
                        FechaFin = soli.FechaFin,
                        Descripcion = soli.Descripcion,
                        ClienteId = soli.ClienteId,
                        ServicioId = soli.ServicioId,
                        EstadoSoliciud = soli.EstadoSoliciud,
                        EmpleadosNombres = textonNombres
                    };
                    lSolicitudes.Add(solicitud);
                }
                return Json(new { status = true, data = lSolicitudes });

            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }
        }

        [Authorize()]
        public async Task<IActionResult> GuardarTarea(SolicitudViewModel solicitudViewModel)
        {
            try
            {
                Tarea tarea = new Tarea
                {
                    FechaInicioTarea = solicitudViewModel.FechaInicioTarea,
                    FechaFinTarea = solicitudViewModel.FechaFinTarea,
                    OperacionId = solicitudViewModel.OperacionId,
                    UsuarioIdentityId = solicitudViewModel.UsuarioIdentityId,
                    DescripcionTarea = solicitudViewModel.DescripcionTarea
                };

                var tareaGuardada = await _solicitudesBuseness.GuardarTarea(tarea);

                if (tareaGuardada != null)
                {
                    return Json(new { status = true, data = tareaGuardada});
                }

                return Json(new { status = false});
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }
        [Authorize()]
        public async Task<IActionResult> Listar()
        {

            try
            {
                var solicitudes = await _solicitudesBuseness.ObtenerListaSolicitudes();

                return Json(new { status = true, data = solicitudes });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }
        [Authorize()]
        public async Task<IActionResult> ObtenerListaSolicitudesPorCliente(int clienteId)
        {

            try
            {

                var solicitudes = await _solicitudesBuseness.ObtenerListaSolicitudesPorCliente(clienteId);

                return Json(new { status = true, data = solicitudes });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }

        [Authorize()]
        public async Task<IActionResult> ObtenerListaSolicitudesPorEmpleado(string id)
        {

            try
            {

                var solicitudes = await _solicitudesBuseness.ObtenerListaSolicitudesPorEmpleado(id);

                return Json(new { status = true, data = solicitudes });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }
       [Authorize()]
        public async Task<IActionResult> ObtenerListaSolicitudesPorServicio(int servicioId)
        {

            try
            {

                var solicitudes = await _solicitudesBuseness.ObtenerListaSolicitudesPorServicio(servicioId);

                return Json(new { status = true, data = solicitudes });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }
        [Authorize()]
        public async Task<IActionResult> ObtenerListaSolicitudesPorEstado(byte estado)
        {
            try
            {
                var solicitudes = await _solicitudesBuseness.ObtenerListaSolicitudesPorEstado(estado);

                return Json(new { status = true, data = solicitudes });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }
        [Authorize()]
        public async Task<IActionResult> ObtenerDetalle(int? id)
        {

            try 
            {
            

            if (id == null)
            {
                return NotFound();
            }

            var solicitud = await _solicitudesBuseness.ObtenerSolicitudPorId(id.Value);

            if (solicitud == null)
            {
                return NotFound();
            }


            var tareas = await _tareasBusiness.ObtenerTareasPorSolicitudId(id.Value);
                var tareasVM = new List<TareaViewModel>();
                foreach (Tarea tarea in tareas)
                {

                    var operacion = await _tareasBusiness.obtenerOperacionPorId(tarea.OperacionId);
                    var empleado =  await _userManager.FindByIdAsync(tarea.UsuarioIdentityId);
                    var tvm = new TareaViewModel
                    {
                        TareaId = tarea.TareaId,
                        nombreOperacion = operacion.Nombre,
                        nombreEmpleado = empleado.Nombre,
                        fechaInicioTarea = tarea.FechaInicioTarea,
                        fechaFinTarea = tarea.FechaFinTarea
                    };
                    tareasVM.Add(tvm);
                }

            SolicitudViewModel solicitudVM = new SolicitudViewModel
            {
                SolicitudId = solicitud.SolicitudId,
                FechaInicio = solicitud.FechaInicio,
                FechaFin = solicitud.FechaFin,
                Descripcion = solicitud.Descripcion,
                ClienteId = solicitud.ClienteId,
                ServicioId = solicitud.ServicioId,
                EstadoSoliciud = solicitud.EstadoSoliciud,
                Tareas = tareasVM
            };

            return Json(new { status = true, data = solicitudVM });

            }
            catch (Exception e)
            {
                return Json(new { status = false});
            }
        }

        [HttpPost]
        [Authorize()]
        public async Task<IActionResult> Edit(Solicitud solicitud)
        {
           
            if (ModelState.IsValid)
            {
                try
                {
                    var tareas =  await _tareasBusiness.ObtenerTareasPorSolicitudId(solicitud.SolicitudId);

                    var fechasDeInicioTareas = new List<DateTime>();
                    var fechasDefinTareas = new List<DateTime>();

                    foreach (var t in tareas)
                    {
                        fechasDeInicioTareas.Add(t.FechaInicioTarea);
                        fechasDefinTareas.Add(t.FechaFinTarea);
                    }

          
                    fechasDeInicioTareas.Sort((x, y) => x.CompareTo(y));
                    fechasDefinTareas.Sort((x, y) => x.CompareTo(y));
 
                    solicitud.FechaFin = fechasDefinTareas[fechasDefinTareas.Count - 1];
                    solicitud.FechaInicio = fechasDeInicioTareas[0];
                    var respuesta = await _solicitudesBuseness.EditarSolicitud(solicitud);
                    if (respuesta)
                    {
                        return Json(new { status = true});
                    }
                    else
                    {
                        return Json(new { status = false });
                    }

                }
                catch (Exception)
                {

                    return Json(new { data = "error" });
                }
            }
            return Json(new { data = "error" });
        }
        [Authorize()]
        public async Task<IActionResult> Guardar(string Descripcion, int ClienteId, int ServicioId, Tarea[] lTareas)
            {

            try
            {

                var fechasDeInicioTareas = new List<DateTime>();
                var fechasDefinTareas = new List<DateTime>();

                foreach (var t in lTareas)
                {
                    fechasDeInicioTareas.Add(t.FechaInicioTarea);
                    fechasDefinTareas.Add(t.FechaFinTarea);
                }

                fechasDefinTareas.OrderByDescending(e => e).ToList();
                fechasDeInicioTareas.OrderByDescending(e => e).ToList();
                
                Solicitud solicitud = new Solicitud
                {                    
                    FechaInicio = fechasDeInicioTareas[0],
                    FechaFin =  fechasDefinTareas[fechasDefinTareas.Count - 1],
                    Descripcion = Descripcion,
                    ClienteId = ClienteId,
                    ServicioId = ServicioId,
                    Tareas = lTareas
                };

                var respuesta = await _solicitudesBuseness.GuardarSolicitud(solicitud);
                if (respuesta)
                {
                    return Json(new { status = true });
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
