using FitAplikacja.Core.Models;
using FitAplikacja.Infrastructure;
using FitAplikacja.Infrastructure.Repositories.Abstract;
using FitAplikacja.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitAplikacja.Services.Concrete
{
    public class AssignedProductService : IAssignedProductService
    {
        private readonly AppDbContext _context;
        private readonly IProductRepository _productRepo;
        private readonly IAssignedProductRepository _assignedProductRepo;

        public AssignedProductService(AppDbContext context, IProductRepository productRepository, IAssignedProductRepository assignedProductRepository)
        {
            _context = context;
            _productRepo = productRepository;
            _assignedProductRepo = assignedProductRepository;
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            return await _productRepo.GetOneAsync(productId);
        }

        public async Task<AssignedProduct> GetAssignedProductAsync(string userId, DateTime date, int assignedProductId)
        {
            return await _context.AssignedProducts
                .Where(a => a.ApplicationUserId == userId)
                .Where(a => a.Added.Date == date.Date)
                .FirstOrDefaultAsync(a => a.Id == assignedProductId);
        }

        public async Task<bool> UnassignProductAsync(AssignedProduct assignedProduct)
        {
            return await _assignedProductRepo.DeleteAsync(assignedProduct);
        }

        public async Task<IEnumerable<AssignedProduct>> GetAssignedProductsAsync(int skip, int take, string userId, DateTime? date)
        {
            IQueryable<AssignedProduct> query;

            query = _context.AssignedProducts.Where(p => p.ApplicationUserId == userId);

            if (date != null)
            {
                query = query.Where(p => p.Added.Date == date.Value.Date);
            }

           return await query.OrderBy(p => p.Id)
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
        }

        public async Task<bool> AssignProductAsync(string userId, Product product, DateTime date, int? weight, int count)
        {
            if (product == null)
            {
                return false;
            }

            var assignedProduct = await _assignedProductRepo.GetOneAsync(product.Id, userId, date);

            if (assignedProduct == null)
            {
                assignedProduct = new AssignedProduct()
                {
                    Added = date.Date,
                    ApplicationUserId = userId,
                    Weight = weight == null ? product.Weight : weight,
                    Count = count
                };

                assignedProduct.Product = product;

                var saveResult = await _assignedProductRepo.SaveAsync(assignedProduct);

                if (!saveResult)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
