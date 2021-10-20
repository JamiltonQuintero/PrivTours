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

        public async Task<IActionResult> ObtenerListaEmpleados()

        {
           try
            {
                var empleados = await _solicitudesBuseness.ObtenerListaEmpleados();
                return Json(new { status = true, data = empleados });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }

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

        public async Task<IActionResult> Guardar(Solicitud solicitud)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false });
            }

            try 
            {
                var respuesta =  await _solicitudesBuseness.GuardarSolicitud(solicitud);
                if(respuesta)
                {
                    return Json(new { status = true });
                } else
                {
                    return Json(new { status = false });
                }
                
            }
            catch (Exception e)
            {
                return Json(new { status = false });
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


        public async Task<IActionResult> ObtenerListaSolicitudesPorEmpleado(int empleadoId)
        {

            try
            {

                var solicitudes = await _solicitudesBuseness.ObtenerListaSolicitudesPorEmpleado(empleadoId);

                return Json(new { status = true, data = solicitudes });
            }
            catch (Exception e)
            {
                return Json(new { status = false });
            }

        }

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

        public async Task<IActionResult> ObtenerDetalle(int? id)
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
            return Json(new { data = solicitud });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Solicitud solicitud)
        {
           
            if (ModelState.IsValid)
            {
                try
                {
                    
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

    }
}
