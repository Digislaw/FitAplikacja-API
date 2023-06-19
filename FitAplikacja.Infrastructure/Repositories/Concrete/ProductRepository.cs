using FitAplikacja.Core.Models;
using FitAplikacja.Infrastructure.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitAplikacja.Infrastructure.Repositories.Concrete
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Product>> SearchAsync(
            string name = null,
            string barcode = null)
        {
            IQueryable<Product> query = entities;

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(barcode))
            {
                query = query.Where(p => p.Barcode.Equals(barcode));
            }

            return await query.ToListAsync();
        }
    }
}
