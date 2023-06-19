using FitAplikacja.Core.Dtos.Input.Exercises;
using FluentValidation;

namespace FitAplikacjaAPI.Validators.Exercises
{
    public class ExerciseRequestValidator : AbstractValidator<ExerciseRequest>
    {
        public ExerciseRequestValidator()
        {
            RuleFor(e => e.Name).NotEmpty().MaximumLength(50);
            RuleFor(e => e.BurnedCalories).NotNull().GreaterThan(0).LessThanOrEqualTo(5000);
            RuleFor(e => e.IsWeightTraining).NotNull();
            RuleFor(e => e.Difficulty).NotNull().LessThanOrEqualTo((byte)5);
            RuleFor(e => e.Series).NotNull().LessThanOrEqualTo((byte)50);
            RuleFor(e => e.Repetition).NotNull().LessThanOrEqualTo((byte)50);
            RuleFor(e => e.Weight).NotNull().LessThanOrEqualTo(500);
            RuleFor(e => e.VideoURL).NotEmpty().MaximumLength(60);
            RuleFor(e => e.Description).NotEmpty().MaximumLength(300);
            RuleFor(e => e.BodyPart).NotEmpty().MaximumLength(50);
        }
    }
}
