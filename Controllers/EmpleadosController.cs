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
    public class EmpleadosController : Controller
    {
        private readonly IEmpleadosBusiness _empleadosBusiness;

        public EmpleadosController(IEmpleadosBusiness empleadosBusiness)
        {
            _empleadosBusiness = empleadosBusiness;
        }

        // GET: Empleados
        public async Task<IActionResult> Index()

        {
            return View(await _empleadosBusiness.ObtenerListaEmpleados());
        }


        
        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _empleadosBusiness.ObtenerEmpleadoPorId(id.Value);
                
            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }

        
        // GET: Empleados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpleadoId,Nombre")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _empleadosBusiness.GuardarEmpleado(empleado);
                    return Json(new { data = "ok" });
                }
                catch (Exception)
                {
                    return Json(new { data = "error" });
                }
            }
            return Json(new { data = "error" });
        }

        
        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _empleadosBusiness.ObtenerEmpleadoPorId(id.Value);
            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }

        // POST: Empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmpleadoId,Nombre")] Empleado empleado)
        {
            if (id != empleado.EmpleadoId)
            {
                return Json(new { data = "error" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _empleadosBusiness.EditarEmpleado(empleado);
                    return Json(new { data = "ok" });

                }
                catch (Exception)
                {

                    return Json(new { data = "error" });
                }
            }
            return Json(new { data = "error" });
        }

        
        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { data = "error", message = "Id no encontrado" });
            }
            try
            {
                var empleado = await _empleadosBusiness.ObtenerEmpleadoPorId(id.Value);
                if (empleado == null)

                    return Json(new { data = "error", message = "Empleado a eliminar no existe" });
                await _empleadosBusiness.EliminarEmpleado(empleado);

                return Json(new { data = "ok", message = "Empleado " + empleado.Nombre + " fue eliminado correctamente" });
            }
            catch (Exception)
            {
                //return RedirectToAction("Error", "Admin");
                return Json(new { data = "error", message = "Ocurrió un error al eliminar el cliente" });
            }
        }
    }
  
}
