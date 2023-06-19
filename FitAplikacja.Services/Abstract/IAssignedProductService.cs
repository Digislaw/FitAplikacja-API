using FitAplikacja.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAplikacja.Services.Abstract
{
    public interface IAssignedProductService
    {
        Task<Product> GetProductAsync(int productId);
        
        Task<AssignedProduct> GetAssignedProductAsync(string userId, DateTime date, int assignedProductId);
        Task<IEnumerable<AssignedProduct>> GetAssignedProductsAsync(int skip, int take, string userId, DateTime? date);
        
        Task<bool> AssignProductAsync(string userId, Product product, DateTime date, int? weight, int count);
        Task<bool> UnassignProductAsync(AssignedProduct assignedProduct);
    }
}
