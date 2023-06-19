using FitAplikacja.Core.Models;
using FitAplikacja.Infrastructure.Repositories.Abstract;

namespace FitAplikacja.Infrastructure.Repositories.Concrete
{
    public class ExerciseRepository : BaseRepository<Exercise>, IExerciseRepository
    {
        public ExerciseRepository(AppDbContext context)
            : base(context) { }
    }
}
