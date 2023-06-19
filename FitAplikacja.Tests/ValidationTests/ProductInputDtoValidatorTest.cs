using FitAplikacjaAPI.Validators.Products;
using FluentValidation.TestHelper;
using Xunit;

namespace FitAplikacja.Tests.ValidationTests
{
    public class ProductInputDtoValidatorTest
    {
        private readonly ProductRequestValidator _validator = new ProductRequestValidator();

        #region Name

        [Fact]
        public void Should_HaveError_When_NameIsNull()
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Name, (string)null);
        }

        [Fact]
        public void Should_HaveError_When_NameIsEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Name, "");
        }

        [Fact]
        public void Should_HaveError_When_NameLengthGreaterThan70()
        {
            string name = new string('A', 71);
            _validator.ShouldHaveValidationErrorFor(p => p.Name, name);
        }

        [Theory]
        [InlineData(70)]
        [InlineData(69)]
        public void Should_NotHaveError_When_NameLengthLessThanOrEqual70(int length)
        {
            string name = new string('A', length);
            _validator.ShouldNotHaveValidationErrorFor(p => p.Name, name);
        }

        #endregion

        #region Calories

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_HaveError_When_CaloriesIsLessThanOrEqual0(int calories)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Calories, calories);
        }

        [Fact]
        public void Should_HaveError_When_CaloriesIsGreaterThan5000()
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Calories, 5001);
        }

        [Fact]
        public void Should_NotHaveError_When_CaloriesIsGreaterThan0()
        {
            _validator.ShouldNotHaveValidationErrorFor(p => p.Calories, 1);
        }

        #endregion
    }
}
