using Dapper;
using Data.Context;
using Data.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.ContainerRepo
{
    public class ContainerRepository : GenericRepository<Container>, IContainerRepository
    {

        public ContainerRepository(WasteCollectionDbContext context, ILogger logger) : base(context, logger)
        {

        }


        public async Task<bool> Add(Container entity)
        {
            var sql = "Insert into Container (ContainerName, Latitude, Longitude, VehicleId) VALUES (@ContainerName, @Latitude, @Longitude, @VehicleId)";
            using (var connection = new SqlConnection("Server=DESKTOP-STUV1FN; Database=Bootcamp; Trusted_Connection=True;"))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return true;
            }
        }

        public async Task<bool> Delete(long id)
        {
            var sql = "DELETE FROM Container WHERE Id = @Id";
            using (var connection = new SqlConnection("Server=DESKTOP-STUV1FN; Database=Bootcamp; Trusted_Connection=True;"))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return true;
            }
        }

        public Task<IEnumerable<Container>> GetAll()
        {
            return base.GetAll();
        }



        public async Task<bool> Update(Container entity)
        {
            var model = await dbSet.FindAsync(entity.Id);
            if(model.VehicleId != entity.VehicleId)
            {
                return false;
            }
            model.ContainerName = entity.ContainerName;
            model.Latitude = entity.Latitude;
            model.Longitude = entity.Longitude;
            return true;
        }

        public async Task<Container> GetById(long id)
        {
            var model = await dbSet.FindAsync(id);
            return model;
        }
    }
}
