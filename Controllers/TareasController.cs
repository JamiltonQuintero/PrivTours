using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using PrivTours.Models.Entities;
using PrivTours.Models.Enums;
using PrivTours.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrivTours.Controllers
{
    public class TareasController : Controller
    {

        private readonly ISolicitudesBusiness _solicitudesBuseness;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly IClientesBusiness _clientesBusiness;
        private readonly IServiciosBusiness _serviciosBusiness;
        private readonly ITareasBusiness _tareasBusiness;
        private readonly SignInManager<UsuarioIdentity> _signInManager;
        public TareasController(UserManager<UsuarioIdentity> userManager, SignInManager<UsuarioIdentity> signInManager, ISolicitudesBusiness solicitudesBuseness, IClientesBusiness clientesBusiness, IServiciosBusiness serviciosBusiness, ITareasBusiness tareasBusiness)
        {
            _solicitudesBuseness = solicitudesBuseness;
            _userManager = userManager;
            _signInManager = signInManager;
            _clientesBusiness = clientesBusiness;
            _serviciosBusiness = serviciosBusiness;
            _tareasBusiness = tareasBusiness;
        }

        // GET: TareasController
        public async Task<ActionResult> Index()
        {
            var solicitudesActivas = new List<Solicitud>();
            
            var usuarioLogeado = await ObtenerUsuarioLogeado();

            var solicitudes = new List<Solicitud>();

            if (usuarioLogeado.Id != null)
            {
                if (usuarioLogeado.RolSeleccionado.ToUpper() == "ADMINISTRADOR")
                {
                    solicitudes = await _solicitudesBuseness.ObtenerListaSolicitudesSVM();
                } else if (usuarioLogeado.RolSeleccionado.ToUpper() == "EMPLEADO")
                {
                    solicitudes = await _solicitudesBuseness.ObtenerListaSolicitudesPorEmpleadoTareas(usuarioLogeado.Id);
                } 
            } 

            foreach (Solicitud solicitud in solicitudes)
            {
                if (solicitud.EstadoSoliciud == (byte)EEstadoSolicitud.RESERVADO ||
                    solicitud.EstadoSoliciud == (byte)EEstadoSolicitud.EN_PROCESO)
                {
                    solicitudesActivas.Add(solicitud);
                }
            }

            var lSolicitudes = new List<SolicitudViewModel>();

            foreach (var solicitud in solicitudesActivas)
            {
                var cliente = await _clientesBusiness.ObtenerClientePorId(solicitud.ClienteId);
                var servicio = await _serviciosBusiness.ObtenerServicioPorId(solicitud.ServicioId);
            SolicitudViewModel solicitudVM = new SolicitudViewModel
                {
                    
                    SolicitudId = solicitud.SolicitudId,
                    FechaInicio = solicitud.FechaInicio,
                    FechaFin = solicitud.FechaFin,
                    Descripcion = solicitud.Descripcion,
                    Cliente = cliente,
                    Servicio = servicio,
                    EstadoSoliciud = solicitud.EstadoSoliciud,
                };
                lSolicitudes.Add(solicitudVM);
            }
            return View(lSolicitudes);
        }

        private async Task<List<string>> ObtenerRolUsuario(UsuarioIdentity usuario)
        {
            return new List<string>(await _userManager.GetRolesAsync(usuario));
        }

        [HttpPost]
        public async Task<ActionResult> CambiarEstadoSolicitud(int id, int tipo)
        {
            try
            {

                var usuarioLogeado = await ObtenerUsuarioLogeado();

                var solicitud = await _solicitudesBuseness.ObtenerSolicitudPorId(id);

                if (usuarioLogeado != null)
                {              
                if (tipo == 1)
                {
                    solicitud.EstadoSoliciud = (byte)EEstadoSolicitud.CANCELADO;
                }
                else if (tipo == 2)
                {

                    if (usuarioLogeado.RolSeleccionado.ToUpper() == "ADMINISTRADOR")
                    {
                        solicitud.EstadoSoliciud = (byte)EEstadoSolicitud.FINALIZADO_ADMIN;
                    }
                    else if (usuarioLogeado.RolSeleccionado.ToUpper() == "EMPLEADO")
                    {
                        solicitud.EstadoSoliciud = (byte)EEstadoSolicitud.FINALIZADO_EMPLEADO;
                    }
                }
                else
                {
                    solicitud.EstadoSoliciud = (byte)EEstadoSolicitud.EN_PROCESO;
                }

                var respuesta = await _solicitudesBuseness.EditarSolicitudEstado(solicitud);
                if (respuesta)
                {
                    return Json(new { status = true });
                }
                else
                {
                    return Json(new { status = false });
                }
            }

                return Json(new { status = false });

            }
            catch (Exception)
            {

                return Json(new { data = "error" });
            }
        }

        private async Task<UsuarioViewModel> ObtenerUsuarioLogeado()
        {
            var usuarioViewModel = new UsuarioViewModel();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null && userId !="")
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

        public async Task<IActionResult> ObtenerTareasPorEmpleadoId(string empleadoId)
        {
            try
            {
            var tareasActivas = new List<Tarea>();

            var tareas = new List<Tarea>();

                tareas = await _tareasBusiness.ObtenerListaTareasPorEmpleadoId(empleadoId);
 
            foreach (Tarea tarea in tareas)
            {

                    tareasActivas.Add(tarea);
                
            }

            var lTareas = new List<SolicitudViewModel>();

            foreach (var t in tareasActivas)
            {

                SolicitudViewModel solicitudVM = new SolicitudViewModel
                {
                    TareaId = t.TareaId,
                    FechaInicioTarea = t.FechaInicioTarea,
                    FechaFinTarea = t.FechaFinTarea,
                    DescripcionTarea = t.DescripcionTarea,
                };

                    lTareas.Add(solicitudVM);
            }

            if (lTareas != null)
            {
                return Json(new { status = true, data = lTareas });
            } else
            {
                return Json(new { status = false});
            }

            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }

        public async Task<IActionResult> ObtenerTareasPorSolicitudId(int solicitudId)
        {

            try
            {
                var tareas = new List<Tarea>();
                var lTareas = new List<SolicitudViewModel>();
                tareas = await _tareasBusiness.ObtenerTareasPorSolicitudId(solicitudId);

                foreach (var t in tareas)
                {
                    var operacion = await _tareasBusiness.obtenerOperacionPorId(t.OperacionId);

                    SolicitudViewModel solicitudVM = new SolicitudViewModel
                    {
                        TareaId = t.TareaId,
                        FechaInicioTarea = t.FechaInicioTarea,
                        FechaFinTarea = t.FechaFinTarea,
                        DescripcionTarea = t.DescripcionTarea,
                        OperacionId = t.OperacionId,
                        Operacion = operacion,
                        UsuarioIdentityId = t.UsuarioIdentityId,
                    };

                    lTareas.Add(solicitudVM);
                }

                if (lTareas != null)
                {
                    return Json(new { status = true, data = lTareas });
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

        public async Task<IActionResult> ObtenerTareaPorId(int id)
        {

            try
            {

                var t = await _tareasBusiness.ObtenerTareaPorId(id);

                    SolicitudViewModel solicitudVM = new SolicitudViewModel
                    {
                        TareaId = t.TareaId,
                        FechaInicioTarea = t.FechaInicioTarea,
                        FechaFinTarea = t.FechaFinTarea,
                        DescripcionTarea = t.DescripcionTarea,
                        OperacionId = t.OperacionId,
                        UsuarioIdentityId = t.UsuarioIdentityId,
                        SolicitudId = t.SolicitudId,
                        EstadoTarea = t.EstadoTarea

                    };


                if (solicitudVM != null)
                {
                    return Json(new { status = true, data = solicitudVM });
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

        public async Task<IActionResult> Edit(Tarea tarea)
        {
            try
            {
               var t = await _tareasBusiness.EditarTarea(tarea);

                if (t)
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

        public async Task<IActionResult> Guardar(Tarea tarea)
        {
            try
            {
                var t = await _tareasBusiness.GuardarTarea(tarea);

                if (t)
                {

                    var operacion = await _tareasBusiness.obtenerOperacionPorId(tarea.OperacionId);

                    var tvm = new TareaViewModel
                    {
                        TareaId = tarea.TareaId,
                        nombreOperacion = operacion.Nombre
                    };

                    return Json(new { status = true, data = tvm });
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

        public async Task<IActionResult> EliminarTareaPorId(int id)
        {
            try
            {               
                var t = await _tareasBusiness.EliminarTareaPorId(id);

                if (t)
                {
                    return Json(new { status = true, data = id });
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
