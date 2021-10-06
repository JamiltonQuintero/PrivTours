using PrivTours.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrivTours.Models.Abstract
{
    public interface IClientesBusiness
    {
        
        Task<IEnumerable<Cliente>> ObtenerListaClientes();
        Task<Cliente> ObtenerClientePorId(int id);
        Task GuardarCliente(Cliente cliente);
        Task EditarCliente(Cliente cliente);
        Task EliminarCliente(Cliente cliente);

    }
}
