using FitAplikacja.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAplikacja.Infrastructure.Repositories.Abstract
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<IEnumerable<Product>> SearchAsync(string name = null, string barcode = null);
    }
}
