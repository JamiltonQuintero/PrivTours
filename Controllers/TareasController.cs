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
            var tareasActivas = new List<Tarea>();
            
            var usuarioLogeado = await ObtenerUsuarioLogeado();

            var tareas = new List<Tarea>();

            if (usuarioLogeado.Id != null)
            {
                if (usuarioLogeado.RolSeleccionado.ToUpper() == "ADMINISTRADOR")
                {
                    tareas = await _tareasBusiness.ObterTareas();

                } else if (usuarioLogeado.RolSeleccionado.ToUpper() == "EMPLEADO")
                {
                    tareas = await _tareasBusiness.ObtenerListaTareasPorEmpleadoId(usuarioLogeado.Id);
                } 
            } 

            foreach (Tarea tarea in tareas)
            {
                if (tarea.EstadoTarea == (byte)EEstadoTarea.RESERVADA ||
                    tarea.EstadoTarea == (byte)EEstadoTarea.INICIADA)
                {
                    var solicitud = await _solicitudesBuseness.ObtenerSolicitudPorId(tarea.SolicitudId);
                    if (solicitud.EstadoSoliciud == (byte)EEstadoSolicitud.RESERVADO ||
                        solicitud.EstadoSoliciud == (byte)EEstadoSolicitud.EN_PROCESO)
                    {
                        tareasActivas.Add(tarea);
                    }                  
                }
            }

            var lSolicitudes = new List<SolicitudViewModel>();

            foreach (var tarea in tareasActivas)
            {
                var solicitud = await _solicitudesBuseness.ObtenerSolicitudPorId(tarea.SolicitudId);
                var cliente = await _clientesBusiness.ObtenerClientePorId(solicitud.ClienteId);
                var servicio = await _serviciosBusiness.ObtenerServicioPorId(solicitud.ServicioId);
                var operacion = await _tareasBusiness.obtenerOperacionPorId(tarea.OperacionId);
            SolicitudViewModel solicitudVM = new SolicitudViewModel
                {
                    
                    TareaId = tarea.TareaId,
                    FechaInicioTarea = tarea.FechaInicioTarea,
                    FechaFinTarea = tarea.FechaFinTarea,
                    DescripcionTarea = tarea.DescripcionTarea,
                    Cliente = cliente,
                    Servicio = servicio,
                    Operacion = operacion,
                    EstadoTarea = tarea.EstadoTarea
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

                var tarea = await _tareasBusiness.ObtenerTareaPorId(id);

                if (usuarioLogeado != null)
                {              
                if (tipo == 1)
                {
                        tarea.EstadoTarea = (byte)EEstadoTarea.CANCELADA;
                }
                else if (tipo == 2)
                {

                    if (usuarioLogeado.RolSeleccionado.ToUpper() == "ADMINISTRADOR")
                    {
                            tarea.EstadoTarea = (byte)EEstadoTarea.FINALIZADA_ADMIN;
                    }
                    else if (usuarioLogeado.RolSeleccionado.ToUpper() == "EMPLEADO")
                    {
                            tarea.EstadoTarea = (byte)EEstadoTarea.FINALIZADA_EMPLEADO;
                    }
                }
                else
                {
                        tarea.EstadoTarea = (byte)EEstadoTarea.INICIADA;
                }

                var respuesta = await _tareasBusiness.EditarTareaEstado(tarea);
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

                return Json(new { data = false });
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
