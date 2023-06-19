using FitAplikacja.Core.Interfaces;
using FitAplikacja.Infrastructure.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitAplikacja.Infrastructure.Repositories.Concrete
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class, IEntity
    {
        protected readonly AppDbContext context;
        protected readonly DbSet<T> entities;

        public BaseRepository(AppDbContext context)
        {
            this.context = context;
            this.entities = context.Set<T>();
        }

        public async Task<T> GetOneAsync(int id)
        {
            return await entities.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<T>> GetManyAsync(int skip = 0, int take = 20)
        {
            return await entities.OrderBy(e => e.Id).Skip(skip).Take(take).ToListAsync();
        }
        public async Task<bool> SaveAsync(T entity)
        {
            if (entity == null)
                return false;

            try
            {
                context.Entry(entity)
                    .State = entity.Id == default ? EntityState.Added : EntityState.Modified;

                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            if (entity == null)
            {
                return false;
            }

            entities.Remove(entity);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
