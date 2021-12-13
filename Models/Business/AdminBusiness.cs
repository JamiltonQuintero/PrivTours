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

        public DateTime fechaInicioTarea { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime fechaFinTarea { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

       

        public Task<List<Tarea>> ObterTareas()
        {
            throw new NotImplementedException();
        }

        public ReporteDashboardViewModel ReporteDashboar()

        {
            ReporteDashboardViewModel reporte = new ReporteDashboardViewModel
            {
                TotCliente = _dbContext.Clientes.Count(),
                TotEmpleado = 0,
                TotUsuario = _dbContext.UsuariosIdentity.Count(),
                TotServicio = _dbContext.Servicios.Count(),
                TotSolicitudes = _dbContext.Solicitudes.Count(),
            };

            return reporte;
        }

     




    }
}