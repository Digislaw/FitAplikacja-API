using FitAplikacja.Core.Dtos.Input.Users;
using FluentValidation;

namespace FitAplikacjaAPI.Validators.Users
{
    public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequest>
    {
        public UserRegistrationRequestValidator()
        {
            RuleFor(u => u.Email).NotEmpty().EmailAddress();
            RuleFor(u => u.Username).NotEmpty().MinimumLength(5);
            RuleFor(u => u.Password).NotEmpty().MinimumLength(6);
        }
    }
}
