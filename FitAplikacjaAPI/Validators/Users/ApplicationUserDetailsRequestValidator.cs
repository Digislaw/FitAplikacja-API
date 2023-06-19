using FitAplikacja.Core.Dtos.Input.Users;
using FluentValidation;

namespace FitAplikacjaAPI.Validators.Users
{
    public class ApplicationUserDetailsRequestValidator : AbstractValidator<ApplicationUserDetailsRequest>
    {
        public ApplicationUserDetailsRequestValidator()
        {
            RuleFor(u => u.UserName).NotEmpty().MinimumLength(5);
            RuleFor(u => u.Weight).GreaterThan(0).LessThanOrEqualTo(200);
            RuleFor(u => u.TargetWeight).GreaterThan(0).LessThanOrEqualTo(200);
            RuleFor(u => u.Height).GreaterThan(0).LessThan(300);
            RuleFor(u => u.Age).GreaterThan(0).LessThanOrEqualTo(100);
            RuleFor(u => u.Kcal).GreaterThan(0);
        }
    }
}
