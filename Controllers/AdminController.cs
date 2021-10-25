using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrivTours.Models.Abstract;
using Microsoft.AspNetCore.Identity;
using PrivTours.ViewModels;

namespace PrivTours.Controllers
{
    public class AdminController : Controller
    {

        private readonly IAdminBusiness _iAdminBusiness;

        public AdminController(IAdminBusiness adminBusiness)
        {
            _iAdminBusiness = adminBusiness;
        }

        public IActionResult Dashboard()
        {
            var reporte = _iAdminBusiness.ReporteDashboar();
            return View(reporte);
        }

    }
}
