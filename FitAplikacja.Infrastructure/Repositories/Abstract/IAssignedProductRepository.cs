using FitAplikacja.Core.Models;
using System;
using System.Threading.Tasks;

namespace FitAplikacja.Infrastructure.Repositories.Abstract
{
    public interface IAssignedProductRepository : IBaseRepository<AssignedProduct>
    {
        Task<AssignedProduct> GetOneAsync(int productId, string userId, DateTime date);
    }
}
