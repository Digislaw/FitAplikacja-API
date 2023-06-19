using AutoMapper;
using FitAplikacja.Core.Dtos.Input.Products;
using FitAplikacja.Core.Dtos.Output.AssignedProducts;
using FitAplikacja.Core.Models;
using System;

namespace FitAplikacjaAPI.Profiles
{
    public class AssignedProductsProfile : Profile
    {
        public AssignedProductsProfile()
        {
            CreateMap<AssignedProductRequest, AssignedProduct>()
                .ForMember(a => a.Added, d => d.NullSubstitute(DateTime.Now));

            CreateMap<AssignedProduct, AssignedProductResponse>();
        }
    }
}
