using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Abstract
{
    public interface ITareasBusiness
    {

        Task<List<Tarea>> ObtenerListaTareasPorEmpleadoId(string id);

        Task<List<Tarea>> ObtenerTareasPorSolicitudId(int solicitudId);

        Task<Operacion> obtenerOperacionPorId(int operacionId);
        Task<Tarea> ObtenerTareaPorId(int tareaId);
        Task<bool> EditarTarea(Tarea tarea);
        Task<bool> GuardarTarea(Tarea tarea);
        Task<bool> EliminarTareaPorId(int id);        

    }
}
