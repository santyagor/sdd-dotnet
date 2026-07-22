using FluentValidation;

namespace RealtorApi.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyValidator : AbstractValidator<UpdatePropertyRequest>
{
    public UpdatePropertyValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(request => request.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(request => request.Address)
            .NotEmpty()
            .WithMessage("Address is required.");

        RuleFor(request => request.Price)
            .NotNull()
            .WithMessage("Price is required.")
            .GreaterThan(0m)
            .WithMessage("Price must be greater than zero.");

        RuleFor(request => request.Status)
            .NotNull()
            .WithMessage("Status is required.")
            .IsInEnum()
            .WithMessage("Status must be a valid property status.");

        RuleFor(request => request.BedroomCount)
            .NotNull()
            .WithMessage("BedroomCount is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("BedroomCount must be zero or greater.");

        RuleFor(request => request.BathroomCount)
            .NotNull()
            .WithMessage("BathroomCount is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("BathroomCount must be zero or greater.");

        RuleFor(request => request.AreaSquareMeters)
            .NotNull()
            .WithMessage("AreaSquareMeters is required.")
            .GreaterThan(0m)
            .WithMessage("AreaSquareMeters must be greater than zero.");
    }
}
