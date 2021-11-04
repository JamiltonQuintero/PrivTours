﻿using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<UsuarioIdentity> _userManager;
        public SolicitudesController(UserManager<UsuarioIdentity> userManager, ISolicitudesBusiness solicitudesBuseness)
        {
            _solicitudesBuseness = solicitudesBuseness;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users.ToListAsync();
            var listaUsuarios = new List<UsuarioIdentity>();

            foreach (var usuario in usuarios)
            {
                var rol = await ObtenerRolUsuario(usuario);
                if ("Empleado".Equals(rol[0]))
                {

                    listaUsuarios.Add(usuario);
                }
            }

            ViewData["Clientes"] = new SelectList(await _solicitudesBuseness.ObtenerListaClientes(), "ClienteId", "Nombre");
            ViewData["Empleados"] = new SelectList(listaUsuarios, "Id", "Nombre");
            ViewData["Servicios"] = new SelectList(await _solicitudesBuseness.ObtenerListaServicios(), "ServicioId", "Nombre");

            return View();
        }
        private async Task<List<string>> ObtenerRolUsuario(UsuarioIdentity usuario)
        {
            return new List<string>(await _userManager.GetRolesAsync(usuario));
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

                var usuarios = await _userManager.Users.ToListAsync();
                var listaUsuarios = new List<UsuarioIdentity>();

                foreach (var usuario in usuarios)
                {
                    var rol = await ObtenerRolUsuario(usuario);
                    if ("Empleado".Equals(rol[0]) && usuario.LockoutEnd == null)
                    {

                        listaUsuarios.Add(usuario);
                    }
                }

                return Json(new { status = true, data = listaUsuarios });
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

        public async Task<IActionResult> ObtenerSolicitudesValidadndoDisponibilidad(SolicitudViewModel solicitudViewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false });
            }

            try
            {

                var lIdsSolicitudes = await _solicitudesBuseness.ObtenerSolicitudesPorEmpleadosSeleccionados(solicitudViewModel.Empleados);
                var lSolicitudes = new List<SolicitudViewModel>();


                foreach (int i in lIdsSolicitudes)
                {
                    var soli = await _solicitudesBuseness.ObtenerSolicitudPorId(i);
                    var count = 0;
                    var textonNombres = "";
                    var texto = "";
                        
                    foreach (var e in soli.DetalleSolicitudEmpleado)
                    {
                        count++;
                        var usuario = await _userManager.FindByIdAsync(e.UsuarioIdentityId);
                        texto = usuario.Nombre + usuario.Apellido;
                        if (soli.DetalleSolicitudEmpleado.Count == count)
                        {
                           
                            textonNombres += texto + " ";
                        }
                        else
                        {
                            textonNombres += texto + ", ";
                        }
                    }
                    
                    SolicitudViewModel solicitud = new SolicitudViewModel
                    {
                        FechaInicio = soli.FechaInicio,
                        FechaFin = soli.FechaFin,
                        HoraInicio = soli.HoraInicio,
                        HoraFinal = soli.HoraFinal,
                        Descripcion = soli.Descripcion,
                        ClienteId = soli.ClienteId,
                        ServicioId = soli.ServicioId,
                        EstadoSoliciud = soli.EstadoSoliciud,
                        EmpleadosNombres = textonNombres
                    };
                    lSolicitudes.Add(solicitud);
                }
                return Json(new { status = true, data = lSolicitudes });

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

        
        public async Task<IActionResult> ObtenerListaSolicitudesPorEmpleado(string id)
        {

            try
            {

                var solicitudes = await _solicitudesBuseness.ObtenerListaSolicitudesPorEmpleado(id);

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

            var detalleEmpleados = await _solicitudesBuseness.ObtenerDetalleEmpleadoPorSolicitudId(solicitud.SolicitudId);
            var empleadosPorSolicitud = new List<string>();
            foreach (DetalleSolicitudEmpleado d in detalleEmpleados)
            {
                var usuario = await _userManager.FindByIdAsync(d.UsuarioIdentityId);
                if (usuario != null)
                {
                    empleadosPorSolicitud.Add(usuario.Id);
                }

            }

            SolicitudViewModel solicitudVM = new SolicitudViewModel
            {
                SolicitudId = solicitud.SolicitudId,
                FechaInicio = solicitud.FechaInicio,
                FechaFin = solicitud.FechaFin,
                HoraInicio = solicitud.HoraInicio,
                HoraFinal = solicitud.HoraFinal,
                Descripcion = solicitud.Descripcion,
                ClienteId = solicitud.ClienteId,
                ServicioId = solicitud.ServicioId,
                EstadoSoliciud = solicitud.EstadoSoliciud,
                Empleados = empleadosPorSolicitud.ToArray()
            };

            return Json(new { data = solicitudVM });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SolicitudViewModel solicitudViewModel)
        {
           
            if (ModelState.IsValid)
            {
                try
                {
                    Solicitud solicitud = new Solicitud
                    {
                        SolicitudId = solicitudViewModel.SolicitudId,
                        FechaInicio = solicitudViewModel.FechaInicio,
                        FechaFin = solicitudViewModel.FechaFin,
                        HoraInicio = solicitudViewModel.HoraInicio,
                        HoraFinal = solicitudViewModel.HoraFinal,
                        Descripcion = solicitudViewModel.Descripcion,
                        ClienteId = solicitudViewModel.ClienteId,
                        ServicioId = solicitudViewModel.ServicioId,
                        EstadoSoliciud = solicitudViewModel.EstadoSoliciud,
                    };
                    var response = await _solicitudesBuseness.EliminarDetallesEmpleadosPorId(solicitud.SolicitudId);
                    var respuesta = await _solicitudesBuseness.EditarSolicitud(solicitud, solicitudViewModel.Empleados);
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

        public async Task<IActionResult> Guardar(SolicitudViewModel solicitudViewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false });
            }

            try
            {
                Solicitud solicitud = new Solicitud
                {                   
                    FechaInicio = solicitudViewModel.FechaInicio,
                    FechaFin = solicitudViewModel.FechaFin,
                    HoraInicio = solicitudViewModel.HoraInicio,
                    HoraFinal = solicitudViewModel.HoraFinal,
                    Descripcion = solicitudViewModel.Descripcion,
                    ClienteId = solicitudViewModel.ClienteId,
                    ServicioId = solicitudViewModel.ServicioId,
                    EstadoSoliciud = solicitudViewModel.EstadoSoliciud,
                };

                var respuesta = await _solicitudesBuseness.GuardarSolicitud(solicitud, solicitudViewModel.Empleados);
                if (respuesta)
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

    }
}
