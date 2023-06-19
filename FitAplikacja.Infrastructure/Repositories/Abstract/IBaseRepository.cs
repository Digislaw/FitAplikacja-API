using FitAplikacja.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAplikacja.Infrastructure.Repositories.Abstract
{
    public interface IBaseRepository<T> where T : class, IEntity
    {
        Task<T> GetOneAsync(int id);
        Task<IEnumerable<T>> GetManyAsync(int skip, int take);
        Task<bool> SaveAsync(T entity);
        Task<bool> DeleteAsync(T entity);
    }
}
