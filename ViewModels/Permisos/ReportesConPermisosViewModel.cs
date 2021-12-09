using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class ReportesConPermisosViewModel
    {

        public ReporteDashboardViewModel Reporte { set; get; }
        public bool Reporte_Generar_Permiso { set; get; } = false;

    }
}
