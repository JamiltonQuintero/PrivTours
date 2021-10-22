using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivTours.Models.Abstract;
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
        public async Task<IActionResult> Create([Bind("EmpleadoId,Nombre,Apellido,Celular,TipoContrato")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
            try
            {
                empleado.Estado = true;
                await _empleadosBusiness.GuardarEmpleado(empleado);
                TempData["Accion"] = "Crear";
                TempData["Mensaje"] = "El empleado "+empleado.Nombre+ " ha sido creado correctamente.";
                return RedirectToAction("Index");
                
            }
            catch (Exception)
            {
                return View(empleado);
            }

        }

                return View(empleado);
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
        public async Task<IActionResult> Edit(int id, [Bind("EmpleadoId,Nombre,Apellido,Celular,TipoContrato")] Empleado empleado)
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
                    TempData["Accion"] = "Editar";
                    TempData["Mensaje"] = "El empleado " + empleado.Nombre+ " ha sido editado correctamente.";
                    return RedirectToAction("Index");

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

                return Json(new { data = "ok", message = "El empleado " + empleado.Nombre + " fue eliminado correctamente" });
            }
            catch (Exception)
            {
                //return RedirectToAction("Error", "Admin");
                return Json(new { data = "error", message = "Ocurrió un error al eliminar el cliente" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CambiarEstado(int? id)
        {
            if (id == null)
            {
                return Json(new { data = "error", message = "Id no encontrado" });
            }
            try
            {
                var empleado = await _empleadosBusiness.ObtenerEmpleadoPorId(id.Value);
                if (empleado == null)
                    return Json(new { data = "error", message = "Empleado a cambiar estado no existe" });
                if (empleado.Estado)
                {
                    empleado.Estado = false;
                }
                else
                {
                    empleado.Estado = true;
                }
                await _empleadosBusiness.EditarEmpleado(empleado);

                var NuevoEstado = empleado.Estado == true ? "Activado" : "Inactivado";
                return Json(new { data = "ok", message = "Empleado " + empleado.Nombre + " fue " + NuevoEstado + " correctamente" });
            }
            catch (Exception)
            {
                return Json(new { data = "error", message = "Ocurrió un error al cambiar estado al usuario" });
            }
        }

    }
}
