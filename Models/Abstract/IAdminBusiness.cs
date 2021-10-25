using PrivTours.Models.Entities;
using PrivTours.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Abstract
{
    public interface IAdminBusiness
    {
        ReporteDashboardViewModel ReporteDashboar();
    }
}
