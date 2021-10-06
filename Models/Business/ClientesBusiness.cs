using Microsoft.EntityFrameworkCore;
using PrivTours.Models.Abstract;
using PrivTours.Models.DAL;
using PrivTours.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrivTours.Models.Business

{
    public class ClientesBusiness : IClientesBusiness
    {

        private readonly DbContextPriv _dbContext;

        public ClientesBusiness(DbContextPriv context)
        {
            _dbContext = context;
        }

        public async Task<IEnumerable<Cliente>> ObtenerListaClientes()
        {
            return await _dbContext.Clientes.ToListAsync();
        }

        public async Task<Cliente> ObtenerClientePorId(int id)
        {
            return await _dbContext.Clientes.FirstOrDefaultAsync(m => m.ClienteId == id);
        }
        public async Task GuardarCliente(Cliente cliente)
        {
            try
            {
                _dbContext.Add(cliente);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task EditarCliente(Cliente cliente)
        {

            try
            {
                _dbContext.Update(cliente);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task EliminarCliente(Cliente cliente)
        {
            try
            {
                _dbContext.Remove(cliente);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
