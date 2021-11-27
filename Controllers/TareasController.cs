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

        public async Task<ActionResult> ObtenerTareasId()
        {
            var tareasActivas = new List<Tarea>();

            var usuarioLogeado = await ObtenerUsuarioLogeado();

            var tareas = new List<Tarea>();

            if (usuarioLogeado.Id != null)
            {
                if (usuarioLogeado.RolSeleccionado.ToUpper() == "ADMINISTRADOR")
                {
                    tareas = await _tareasBusiness.ObterTareas();

                }
                else if (usuarioLogeado.RolSeleccionado.ToUpper() == "EMPLEADO")
                {
                    tareas = await _tareasBusiness.ObtenerListaTareasPorEmpleadoId(usuarioLogeado.Id);
                }
            }
            var lTareasId = new List<int>();
            foreach (Tarea tarea in tareas)
            {
                if (tarea.EstadoTarea == (byte)EEstadoTarea.RESERVADA ||
                    tarea.EstadoTarea == (byte)EEstadoTarea.INICIADA)
                {
                    var solicitud = await _solicitudesBuseness.ObtenerSolicitudPorId(tarea.SolicitudId);
                    if (solicitud.EstadoSoliciud == (byte)EEstadoSolicitud.RESERVADO ||
                        solicitud.EstadoSoliciud == (byte)EEstadoSolicitud.EN_PROCESO)
                    {
                        lTareasId.Add(tarea.TareaId);
                    }
                }
            }

            return Json(new { status = true, data = lTareasId });

        }

        private async Task<List<string>> ObtenerRolUsuario(UsuarioIdentity usuario)
        {
            return new List<string>(await _userManager.GetRolesAsync(usuario));
        }

        [HttpPost]
        public async Task<ActionResult> CambiarEstadoSolicitud(int id, int tipo, string novedad)
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
                        tarea.Novedad = novedad;
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
                        var tareas = await _tareasBusiness.ObtenerTareasPorSolicitudId(tarea.SolicitudId);
                        var cambiarEstadoSolicitudUltimaTarea = true;
                        foreach (var t in tareas)
                        {
                            if (t.EstadoTarea == (byte)EEstadoTarea.INICIADA ||
                                t.EstadoTarea == (byte)EEstadoTarea.RESERVADA)
                            {
                                cambiarEstadoSolicitudUltimaTarea = false;
                            }
                        }

                        if (cambiarEstadoSolicitudUltimaTarea)
                        {
                            var solicitud = await _solicitudesBuseness.ObtenerSolicitudPorId(tarea.SolicitudId);
                            solicitud.EstadoSoliciud = (byte)EEstadoSolicitud.FINALIZADO;
                            var r = await _solicitudesBuseness.EditarSolicitudEstado(solicitud);
                            if (r)
                            {
                                return Json(new { status = true });
                            } else
                            {
                                return Json(new { status = false });
                            }
                        }

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

        public async Task<IActionResult> ObtenerTareasPorRangoFechaDeInicio(DateTime fechaInicioFiltro, DateTime fechaFinFiltro)
        {
            try
            {
                var tareasActivas = new List<Tarea>();

                var tareas = new List<Tarea>();

                var usuarioLogeado = await ObtenerUsuarioLogeado();

                tareas = await _tareasBusiness.ObtenerListaTareasPorEmpleadoId(usuarioLogeado.Id);

                var result = tareas.FindAll(a => {                
                    return (a.FechaInicioTarea >= fechaInicioFiltro && a.FechaInicioTarea <= fechaFinFiltro);
                });

                foreach (Tarea tarea in result)
                {

                    tareasActivas.Add(tarea);

                }

                var lTareas = new List<SolicitudViewModel>();

                foreach (var t in tareasActivas)
                {
                    var solicitud = await _solicitudesBuseness.ObtenerSolicitudPorId(t.SolicitudId);
                    var cliente = await _clientesBusiness.ObtenerClientePorId(solicitud.ClienteId);
                    var servicio = await _serviciosBusiness.ObtenerServicioPorId(solicitud.ServicioId);
                    var operacion = await _tareasBusiness.obtenerOperacionPorId(t.OperacionId);
                    SolicitudViewModel solicitudVM = new SolicitudViewModel
                    {
                        TareaId = t.TareaId,
                        FechaInicioTarea = t.FechaInicioTarea,
                        FechaFinTarea = t.FechaFinTarea,
                        DescripcionTarea = t.DescripcionTarea, 
                        Cliente = cliente,
                        Servicio = servicio,
                        Operacion = operacion,
                        EstadoTarea = t.EstadoTarea
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

        public async Task<IActionResult> ObtenerListaTareasPorEstado(byte estado)
        {
            try
            {
                var tareas = new List<Tarea>();

                var usuarioLogeado = await ObtenerUsuarioLogeado();

                tareas = await _tareasBusiness.ObtenerListaTareasPorEmpleadoId(usuarioLogeado.Id);
                var tareasPorEstado = tareas.FindAll(tarea => tarea.EstadoTarea == estado);
                var lTareas = new List<SolicitudViewModel>();

                foreach (var t in tareasPorEstado)
                {
                    var solicitud = await _solicitudesBuseness.ObtenerSolicitudPorId(t.SolicitudId);
                    var cliente = await _clientesBusiness.ObtenerClientePorId(solicitud.ClienteId);
                    var servicio = await _serviciosBusiness.ObtenerServicioPorId(solicitud.ServicioId);
                    var operacion = await _tareasBusiness.obtenerOperacionPorId(t.OperacionId);
                    SolicitudViewModel solicitudVM = new SolicitudViewModel
                    {
                        TareaId = t.TareaId,
                        FechaInicioTarea = t.FechaInicioTarea,
                        FechaFinTarea = t.FechaFinTarea,
                        DescripcionTarea = t.DescripcionTarea,
                        Cliente = cliente,
                        Servicio = servicio,
                        Operacion = operacion,
                        EstadoTarea = t.EstadoTarea
                    };

                    lTareas.Add(solicitudVM);
                }

                return Json(new { status = true, data = lTareas });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }

        
    public async Task<IActionResult> ObtenerTareasPorRangoFechaDeFin(DateTime fechaInicioFiltro, DateTime fechaFinFiltro)
        {
            try
            {
                var tareasActivas = new List<Tarea>();

                var tareas = new List<Tarea>();

                var usuarioLogeado = await ObtenerUsuarioLogeado();

                tareas = await _tareasBusiness.ObtenerListaTareasPorEmpleadoId(usuarioLogeado.Id);

                var result = tareas.FindAll(a => {
                    return (a.FechaFinTarea >= fechaInicioFiltro && a.FechaFinTarea <= fechaFinFiltro);
                });

                foreach (Tarea tarea in result)
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
                    var tareas = await _tareasBusiness.ObtenerTareasPorSolicitudId(tarea.SolicitudId);
                    var solicitud = await _solicitudesBuseness.ObtenerSolicitudPorId(tarea.SolicitudId);
                    var fechasDeInicioTareas = new List<DateTime>();
                    var fechasDefinTareas = new List<DateTime>();

                    foreach (var ta in tareas)
                    {
                        fechasDeInicioTareas.Add(ta.FechaInicioTarea);
                        fechasDefinTareas.Add(ta.FechaFinTarea);
                    }

                    fechasDeInicioTareas.Sort((x, y) => x.CompareTo(y));
                    fechasDefinTareas.Sort((x, y) => x.CompareTo(y));
                    solicitud.FechaFin = fechasDefinTareas[fechasDefinTareas.Count - 1];
                    solicitud.FechaInicio = fechasDeInicioTareas[0];
                    var respuesta = await _solicitudesBuseness.EditarSolicitud(solicitud);
                    if (respuesta)
                    {
                        return Json(new { status = true });
                    }
                    else
                    {
                        return Json(new { status = false });
                    }

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

        

    public async Task<IActionResult> ObtenerTareaPorIdEdit(int id)
        {
            try
            {
                var t = await _tareasBusiness.ObtenerTareaPorId(id);

                if (t != null)
                {
                    var operacion = await _tareasBusiness.obtenerOperacionPorId(t.OperacionId);
                    var empleado = await _userManager.FindByIdAsync(t.UsuarioIdentityId);
                    var tvm = new TareaViewModel
                    {
                        TareaId = t.TareaId,
                        nombreOperacion = operacion.Nombre,
                        nombreEmpleado = empleado.Nombre,
                        fechaInicioTarea = t.FechaInicioTarea,
                        fechaFinTarea = t.FechaFinTarea
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

        public async Task<IActionResult> Guardar(Tarea tarea)
        {
            try
            {
                var t = await _tareasBusiness.GuardarTarea(tarea);

                if (t)
                {
                    var tareas = await _tareasBusiness.ObtenerTareasPorSolicitudId(tarea.SolicitudId);
                    var solicitud = await _solicitudesBuseness.ObtenerSolicitudPorId(tarea.SolicitudId);
                    var fechasDeInicioTareas = new List<DateTime>();
                    var fechasDefinTareas = new List<DateTime>();

                    foreach (var ta in tareas)
                    {
                        fechasDeInicioTareas.Add(ta.FechaInicioTarea);
                        fechasDefinTareas.Add(ta.FechaFinTarea);
                    }

                    fechasDeInicioTareas.Sort((x, y) => x.CompareTo(y));
                    fechasDefinTareas.Sort((x, y) => x.CompareTo(y));
                    solicitud.FechaFin = fechasDefinTareas[fechasDefinTareas.Count - 1];
                    solicitud.FechaInicio = fechasDeInicioTareas[0];
                    var respuesta = await _solicitudesBuseness.EditarSolicitud(solicitud);
                    if (respuesta)
                    {

                        var operacion = await _tareasBusiness.obtenerOperacionPorId(tarea.OperacionId);
                        var empleado = await _userManager.FindByIdAsync(tarea.UsuarioIdentityId);
                        var tvm = new TareaViewModel
                        {
                            TareaId = tarea.TareaId,
                            nombreOperacion = operacion.Nombre,
                            nombreEmpleado = empleado.Nombre,
                            fechaInicioTarea = tarea.FechaInicioTarea,
                            fechaFinTarea = tarea.FechaFinTarea
                        };

                        return Json(new { status = true, data = tvm });
                    }
                    else
                    {
                        return Json(new { status = false });
                    }

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
                var tarea = await _tareasBusiness.ObtenerTareaPorId(id);
                

                if (tarea != null)
                {

                    var t = await _tareasBusiness.EliminarTareaPorId(id);

                    if (t)
                    {
                        var tareas = await _tareasBusiness.ObtenerTareasPorSolicitudId(tarea.SolicitudId);
                        var solicitud = await _solicitudesBuseness.ObtenerSolicitudPorId(tarea.SolicitudId);
                        var fechasDeInicioTareas = new List<DateTime>();
                        var fechasDefinTareas = new List<DateTime>();

                        foreach (var ta in tareas)
                        {
                            fechasDeInicioTareas.Add(ta.FechaInicioTarea);
                            fechasDefinTareas.Add(ta.FechaFinTarea);
                        }

                        fechasDeInicioTareas.Sort((x, y) => x.CompareTo(y));
                        fechasDefinTareas.Sort((x, y) => x.CompareTo(y));
                        solicitud.FechaFin = fechasDefinTareas[fechasDefinTareas.Count - 1];
                        solicitud.FechaInicio = fechasDeInicioTareas[0];
                        var respuesta = await _solicitudesBuseness.EditarSolicitud(solicitud);
                        if (respuesta)
                        {
                            return Json(new { status = true, data = tarea.TareaId });
                        }
                        else
                        {
                            return Json(new { status = false });
                        }
                    }
                    else
                    {
                        return Json(new { status = false });
                    }
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
