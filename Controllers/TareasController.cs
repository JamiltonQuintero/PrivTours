using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using PrivTours.Models.Entities;
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
            var solicitudes = await _solicitudesBuseness.ObtenerListaSolicitudesSVM();
            var lSolicitudes = new List<SolicitudViewModel>();

            foreach (var solicitud in solicitudes)
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

        // GET: TareasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TareasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TareasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TareasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TareasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TareasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TareasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
