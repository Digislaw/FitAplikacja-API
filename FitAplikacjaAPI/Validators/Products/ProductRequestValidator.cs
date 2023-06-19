using FitAplikacja.Core.Dtos.Input.Products;
using FluentValidation;

namespace FitAplikacjaAPI.Validators.Products
{
    public class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(p => p.Name).NotEmpty().MaximumLength(70);
            RuleFor(p => p.Barcode).NotEmpty().MaximumLength(100);
            RuleFor(p => p.Calories).NotNull().GreaterThan(0).LessThanOrEqualTo(5000);
            RuleFor(p => p.Carbs).GreaterThan(0).LessThanOrEqualTo(300);
            RuleFor(p => p.Fat).GreaterThan(0).LessThanOrEqualTo(100);
            RuleFor(p => p.Protein).GreaterThan(0).LessThanOrEqualTo(300);
        }
    }
}
