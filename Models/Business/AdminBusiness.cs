using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using PrivTours.Models.DAL;
using PrivTours.Models.Entities;
using PrivTours.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Business
{
    public class AdminBusiness : IAdminBusiness
    {

        private readonly DbContextPriv _dbContext;

        public AdminBusiness(DbContextPriv context)
        {
            _dbContext = context;
        }

        public ReporteDashboardViewModel ReporteDashboar()

        {
            ReporteDashboardViewModel reporte = new ReporteDashboardViewModel
            {
                TotCliente = _dbContext.Clientes.Count(),
                TotEmpleado = _dbContext.Empleados.Count(),
                TotUsuario = _dbContext.UsuariosIdentity.Count(),
                TotServicio = _dbContext.Servicios.Count(),
                TotSolicitudes = _dbContext.Solicitudes.Count(),
            };

            return reporte;
        }

        public async Task<List<Cliente>> ObtenerListaClientes()
        {
            var clientes = await _dbContext.Clientes.ToListAsync();

            return clientes;
        }

        

    }
}
