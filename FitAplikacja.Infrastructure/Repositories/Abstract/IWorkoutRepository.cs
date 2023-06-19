using FitAplikacja.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAplikacja.Infrastructure.Repositories.Abstract
{
    public interface IWorkoutRepository : IBaseRepository<Workout>
    {
        Task<IEnumerable<Workout>> GetManyAsync(int skip, int take, string userId);
    }
}
