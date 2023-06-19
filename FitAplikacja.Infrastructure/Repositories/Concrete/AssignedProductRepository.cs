using FitAplikacja.Core.Models;
using FitAplikacja.Infrastructure.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FitAplikacja.Infrastructure.Repositories.Concrete
{
    public class AssignedProductRepository : BaseRepository<AssignedProduct>, IAssignedProductRepository
    {
        public AssignedProductRepository(AppDbContext context)
            : base(context)
        {

        }

        public async Task<AssignedProduct> GetOneAsync(int productId, string userId, DateTime date)
        {
            return await context.AssignedProducts
                .Where(a => a.ApplicationUserId == userId)
                .Where(a => a.Added.Date == date.Date)
                .FirstOrDefaultAsync(a => a.ProductId == productId);
        }
    }
}
