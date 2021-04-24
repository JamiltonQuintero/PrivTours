using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using PrivTours.Models.DAL;
using PrivTours.Models.Entities;
using PrivTours.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Controllers
{

    
    public class SolicitudesController : Controller
    {


        private readonly ISolicitudesBusiness _solicitudesBuseness;

        public SolicitudesController(ISolicitudesBusiness solicitudesBuseness)
        {
            _solicitudesBuseness = solicitudesBuseness;
        }

        public async Task<IActionResult> Index()
        {

            ViewData["Clientes"] = new SelectList(await _solicitudesBuseness.ObtenerListaClientes(), "ClienteId", "Nombre");
            ViewData["Empleados"] = new SelectList(await _solicitudesBuseness.ObtenerListaEmpleados(), "EmpleadoId", "Nombre");
            ViewData["Servicios"] = new SelectList(await _solicitudesBuseness.ObtenerListaServicios(), "ServicioId", "Nombre");
            return View();
        }


        public async Task<IActionResult> Guardar(Solicitud solicitud)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false });
            }

            try 
            {
                await _solicitudesBuseness.GuardarSolicitud(solicitud);
                return Json(new { status = true });
            }
            catch (Exception e)
            {
                return Json(new { status = true });
            }

        }


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
    }
}
