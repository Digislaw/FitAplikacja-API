using FitAplikacja.Core.Dtos.Input.Products;
using FluentValidation;

namespace FitAplikacjaAPI.Validators.AssignedProducts
{
    public class AssignedProductRequestValidator : AbstractValidator<AssignedProductRequest>
    {
        public AssignedProductRequestValidator()
        {
            RuleFor(a => a.ProductId).NotNull();
            RuleFor(a => a.Date).NotNull();
            RuleFor(a => a.Count).NotNull().GreaterThan(0);
        }
    }
}
