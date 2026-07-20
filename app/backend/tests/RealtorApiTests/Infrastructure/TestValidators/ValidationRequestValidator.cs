using FluentValidation;
using RealtorApiTests.Infrastructure.TestSlices;

namespace RealtorApiTests.Infrastructure.TestValidators
{
    public class ValidationRequestValidator : AbstractValidator<ValidationRequest>
    {
        public ValidationRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Age)
                .GreaterThan(0)
                .WithMessage("Age must be greater than zero.");
        }
    }
}
