using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivTours.ViewModels
{
    public class ReportesConPermisosViewModel
    {

        public ReporteDashboardViewModel Reporte { set; get; }
        public bool Reporte_Generar_Permiso { set; get; } = false;


        
    }
}
