using PrivTours.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrivTours.Models.Abstract
{
    public interface IServiciosBusiness
    {
        Task<IEnumerable<Servicio>> ObtenerListaServicios();
        Task<Servicio> ObtenerServicioPorId(int id);
        Task GuardarServicio(Servicio servicio);
        Task EditarServicio(Servicio servicio);
        Task EliminarServicio(Servicio servicio);

        Task<List<DetallePermiso>> ObtenerPermisosPorRolId(string id);
    }
}
