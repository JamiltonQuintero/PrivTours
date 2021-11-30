using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using PrivTours.Models.DAL;
using PrivTours.Models.Entities;
using PrivTours.Models.Enums;
using PrivTours.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Business
{
    public class TareasBusiness: ITareasBusiness
    {

        private readonly DbContextPriv _dbContext;

        public TareasBusiness(DbContextPriv dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<Tarea>> ObtenerListaTareasPorEmpleadoId(string id)
        {
            var tareas = await _dbContext.Tareas.ToListAsync();
            var tareasEmpleado = tareas.FindAll(t => t.UsuarioIdentityId == id);
            return tareasEmpleado;
        }

        public async Task<List<Tarea>> ObtenerListaTareas()
        {
            var tareas = await _dbContext.Tareas.ToListAsync();
            
            return tareas;
        }

        public async Task<List<Tarea>> ObtenerTareasPorSolicitudId(int solicitudId)
        {

            var tareas= await _dbContext.Tareas.ToListAsync();
            var tareasPorSolicitud = tareas.FindAll(t => t.SolicitudId == solicitudId);

            return tareasPorSolicitud;

        }

        public async Task<List<Tarea>> ObtenerListaTareasPorEstado(byte estado)
        {
            var tareas = await _dbContext.Tareas.ToListAsync();
                
            var tareasPorEstado = tareas.FindAll(tarea => tarea.EstadoTarea == estado);
            return tareasPorEstado;
        }

        public async Task<Operacion> obtenerOperacionPorId(int operacionId)
        {
            return await _dbContext.Operaciones.FirstOrDefaultAsync(o => o.OperacionId == operacionId); 
        }

        public async Task<Tarea> ObtenerTareaPorId(int tareaId) {
            return await _dbContext.Tareas.FirstOrDefaultAsync(t => t.TareaId == tareaId);
        }

        public async Task<bool> EditarTarea(Tarea tarea)
        {

            try {
                _dbContext.Update(tarea);
                await _dbContext.SaveChangesAsync();
                return true; 
            }catch(Exception e)
            {
                return false;
            }
        }

        public async Task<bool> GuardarTarea(Tarea tarea)
        {

            try
            {
                tarea.EstadoTarea = (byte)EEstadoTarea.RESERVADA;
                _dbContext.Add(tarea);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<bool> EliminarTareaPorId(int id)
        {
            try
            {
                var tarea = await _dbContext.Tareas.FirstOrDefaultAsync(t => t.TareaId == id);
                if (tarea != null)
                {
                    _dbContext.Remove(tarea);
                    await _dbContext.SaveChangesAsync();
                    return true;

                }
                else
                {
                    return false;

                }
                
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<List<Tarea>> ObterTareas()
        {
            return await _dbContext.Tareas.ToListAsync();
        }

        public async Task<bool> EditarTareaEstado(Tarea tarea)
        {
            try
            {
                _dbContext.Update(tarea);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
