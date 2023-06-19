using AutoMapper;
using FitAplikacja.Core.Dtos.Input.Products;
using FitAplikacja.Core.Dtos.Output.Products;
using FitAplikacja.Core.Models;

namespace FitAplikacjaAPI.Profiles
{
    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            CreateMap<Product, ProductResponse>();
            CreateMap<ProductRequest, Product>();
        }
    }
}
