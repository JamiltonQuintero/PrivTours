using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivTours.Models.Abstract;
using PrivTours.Models.Entities;

namespace PrivTours.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClientesBusiness _clientesBusiness;

        public ClientesController(IClientesBusiness clientesBusiness)
        {
            _clientesBusiness = clientesBusiness;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            return View(await _clientesBusiness.ObtenerListaClientes());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _clientesBusiness.ObtenerClientePorId(id.Value);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    cliente.Estado = true;
                    await _clientesBusiness.GuardarCliente(cliente);
                    TempData["Accion"] = "Crear";
                    TempData["Mensaje"] = "Se ha creado correctamente el usuario " + cliente.Nombre;
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return View(cliente);
                }
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _clientesBusiness.ObtenerClientePorId(id.Value);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.ClienteId)
            {
                return Json(new { data = "error" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _clientesBusiness.EditarCliente(cliente);
                    TempData["Accion"] = "Editar";
                    TempData["Mensaje"] = "Se ha editado correctamente el cliente " + cliente.Nombre;
                    return RedirectToAction("Index");

                }
                catch (Exception)
                {

                    return Json(new { data = "error" });
                }
            }
            return Json(new { data = "error" });
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { data = "error", message = "Id no encontrado" });
            }
            try
            {
                var cliente = await _clientesBusiness.ObtenerClientePorId(id.Value);
                if (cliente == null)

                    return Json(new { data = "error", message = "Cliente a eliminar no existe" });
                await _clientesBusiness.EliminarCliente(cliente);

                return Json(new { data = "ok", message = "Cliente " + cliente.Nombre + " fue eliminado correctamente" });
            }
            catch (Exception)
            {
                return Json(new { data = "error", message = "Ocurrió un error al eliminar al cliente" });
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
                var cliente = await _clientesBusiness.ObtenerClientePorId(id.Value);
                if (cliente == null)
                    return Json(new { data = "error", message = "Cliente a cambiar estado no existe" });
                if (cliente.Estado)
                {
                    cliente.Estado = false;
                }
                else
                {
                    cliente.Estado = true;
                }
                await _clientesBusiness.EditarCliente(cliente);

                var NuevoEstado = cliente.Estado == true ? "Activado" : "Inactivado";
                return Json(new { data = "ok", message = "Empleado " + cliente.Nombre + " fue " + NuevoEstado + " correctamente" });
            }
            catch (Exception)
            {
                return Json(new { data = "error", message = "Ocurrió un error al cambiar estado al usuario" });
            }
        }

    }
}

