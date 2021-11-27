using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.ViewModels
{
    public class TareaViewModel
    {
        public int TareaId { get; set; }
        public string nombreOperacion { get; set; }
        public string nombreEmpleado { get; set; }
        public DateTime fechaInicioTarea { get; set; }
        public DateTime fechaFinTarea { get; set; }

    }
}
