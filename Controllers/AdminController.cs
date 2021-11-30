using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrivTours.Models.Abstract;
using Microsoft.AspNetCore.Identity;
using PrivTours.ViewModels;
using Microsoft.AspNetCore.Authorization;
using PrivTours.Filters;

namespace PrivTours.Controllers
{
    [NoCache]
    public class AdminController : Controller
    {

        private readonly IAdminBusiness _iAdminBusiness;

        public AdminController(IAdminBusiness adminBusiness)
        {
            _iAdminBusiness = adminBusiness;
        }

        [Authorize()]
        public IActionResult Dashboard()
        {
            var reporte = _iAdminBusiness.ReporteDashboar();
            return View(reporte);
        }

    }
}
