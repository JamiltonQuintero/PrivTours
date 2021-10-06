using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using PrivTours.Models.DAL;
using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrivTours.Models.Business

{
    public class EmpleadosBusiness : IEmpleadosBusiness
    {

        private readonly DbContextPriv _dbContext;

        public EmpleadosBusiness(DbContextPriv context)
        {
            _dbContext = context;
        }

        public async Task<IEnumerable<Empleado>> ObtenerListaEmpleados()
        {
            return await _dbContext.Empleados.ToListAsync();
        }

        public async Task<Empleado> ObtenerEmpleadoPorId(int id) {
            return await _dbContext.Empleados.FirstOrDefaultAsync(m => m.EmpleadoId == id);
        }
        public async Task GuardarEmpleado(Empleado empleado)
        {
            try
            {
                _dbContext.Add(empleado);
            await _dbContext.SaveChangesAsync();
        }
            catch (Exception e)
            {
                throw e;
            }
}
        public async Task EditarEmpleado(Empleado empleado)
        {

            try
            {
                _dbContext.Update(empleado);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task EliminarEmpleado(Empleado empleado)
        {
            try
            {
                _dbContext.Remove(empleado);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
