using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using PrivTours.Models.DAL;
using PrivTours.Models.Entities;

namespace PrivTours.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly IServiciosBusiness _serviciosBusiness;

        public ServiciosController(IServiciosBusiness serviciosBusiness)
        {
            _serviciosBusiness = serviciosBusiness;
        }

        // GET: Servicios
        public async Task<IActionResult> Index()
        {
            return View(await _serviciosBusiness.ObtenerListaServicios());
        }

        // GET: Servicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _serviciosBusiness.ObtenerServicioPorId(id.Value);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        // GET: Servicios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Servicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServicioId,Nombre")] Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _serviciosBusiness.GuardarServicio(servicio);
                    return Json(new { data = "ok" });
                }
                catch (Exception)
                {
                    return Json(new { data = "error" });
                }
            }
            return Json(new { data = "error" });
        }

        // GET: Servicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _serviciosBusiness.ObtenerServicioPorId(id.Value);
            if (servicio == null)
            {
                return NotFound();
            }
            return View(servicio);
        }

        // POST: Servicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServicioId,Nombre")] Servicio servicio)
        {
            if (id != servicio.ServicioId)
            {
                return Json(new { data = "error" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _serviciosBusiness.EditarServicio(servicio);
                    return Json(new { data = "ok" });
                }
                catch (Exception)
                {

                    return Json(new { data = "error" });
                }
            }
            return Json(new { data = "error" });
        }

        // GET: Servicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { data = "error", message = "Id no encontrado" });
            }
            try
            {
                var servicio = await _serviciosBusiness.ObtenerServicioPorId(id.Value);
                if (servicio == null)

                    return Json(new { data = "error", message = "Servicio a eliminar no existe" });
                await _serviciosBusiness.EliminarServicio(servicio);
                return Json(new { data = "ok", message = "Servicio " + servicio.Nombre + " fue eliminado correctamente" });
            }
            catch (Exception)
            {
                return Json(new { data = "error", message = "Ocurrió un error al eliminar al servicio" });
            }
        }
    }
}

