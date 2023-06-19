using FitAplikacja.Core.Models;
using FitAplikacja.Infrastructure.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitAplikacja.Infrastructure.Repositories.Concrete
{
    public class WorkoutRepository : BaseRepository<Workout>, IWorkoutRepository
    {
        public WorkoutRepository(AppDbContext context) 
            : base(context) { }

        public async Task<IEnumerable<Workout>> GetManyAsync(int skip, int take, string userId = null)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                return await context.Workouts
                    .Where(w => w.ApplicationUserId == userId)
                    .OrderBy(p => p.Id)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();
            }

            return System.Array.Empty<Workout>();
        }
    }
}
