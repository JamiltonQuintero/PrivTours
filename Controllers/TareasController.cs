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
using System.Threading.Tasks;

namespace PrivTours.Controllers
{
    public class TareasController : Controller
    {

        private readonly ISolicitudesBusiness _solicitudesBuseness;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly IClientesBusiness _clientesBusiness;
        private readonly IServiciosBusiness _serviciosBusiness;
        public TareasController(UserManager<UsuarioIdentity> userManager, ISolicitudesBusiness solicitudesBuseness, IClientesBusiness clientesBusiness, IServiciosBusiness serviciosBusiness)
        {
            _solicitudesBuseness = solicitudesBuseness;
            _userManager = userManager;
            _clientesBusiness = clientesBusiness;
            _serviciosBusiness = serviciosBusiness;
        }

        // GET: TareasController
        public async Task<ActionResult> Index()
        {
            var solicitudesActivas = new List<Solicitud>();
            var solicitudes = await _solicitudesBuseness.ObtenerListaSolicitudesSVM();

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
                    HoraInicio = solicitud.HoraInicio,
                    HoraFinal = solicitud.HoraFinal,
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
        public async Task<ActionResult> CambiarEstadoSolicitud(int id, int tipo, string rol)
        {
            try
            {

                var solicitud = await _solicitudesBuseness.ObtenerSolicitudPorId(id);

                if(tipo == 1)
                {
                    solicitud.EstadoSoliciud = (byte)EEstadoSolicitud.CANCELADO;
                }else if(tipo == 2)
                {
                    if (rol == "ADMINISTRADOR")
                    {
                        solicitud.EstadoSoliciud = (byte)EEstadoSolicitud.FINALIZADO_ADMIN;
                    } else if(rol == "EMPLEADO")
                    {
                        solicitud.EstadoSoliciud = (byte)EEstadoSolicitud.FINALIZADO_EMPLEADO;
                    }
                }else
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
            catch (Exception)
            {

                return Json(new { data = "error" });
            }


        }

    }
}
