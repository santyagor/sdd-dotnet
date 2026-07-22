using FluentValidation;

namespace RealtorApi.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdValidator : AbstractValidator<GetPropertyByIdRequest>
{
    public GetPropertyByIdValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage("Id is required.")
            .Must(BeValidGuid)
            .WithMessage("Id must be a valid GUID.");
    }

    private static bool BeValidGuid(string? value)
    {
        return Guid.TryParse(value, out _);
    }
}
