using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ILogger logger;
        protected WasteCollectionDbContext context;
        internal DbSet<T> dbSet;


        public GenericRepository ( WasteCollectionDbContext context, ILogger logger)
        {
            this.logger= logger;
            this.context=context;

            dbSet = context.Set<T>();
        }


        public Task<bool> Add(T entity)
        {
            var result = dbSet.Add(entity);
            return Task.FromResult(result.State == EntityState.Added);
        }

        public Task<bool> Delete(long id)
        {
            var entity = GetById(id);
            var result = dbSet.Remove(entity.Result);
            return Task.FromResult(result.State == EntityState.Deleted);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public virtual async Task<T> GetById(long id)
        {
            var model = await dbSet.FindAsync(id);
            return model;
        }

        public Task<bool> Update(T entity)
        {
            var result = dbSet.Update(entity);
            return Task.FromResult(result.State == EntityState.Modified);
        }
    }
}
