using Data.Generic;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Data.VehicleRepo
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        Task<IEnumerable<Container>> GetContainers(long id);
    }
}
