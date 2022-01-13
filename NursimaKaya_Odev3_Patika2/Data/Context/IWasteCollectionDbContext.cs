using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Context
{
    public interface IWasteCollectionDbContext
    {
        DbSet<Vehicle> Vehicle { get; set; }
        DbSet<Container> Container { get; set; }

        int SaveChanges();
    }
}
