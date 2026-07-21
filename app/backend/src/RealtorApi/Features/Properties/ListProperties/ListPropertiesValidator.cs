using FluentValidation;

namespace RealtorApi.Features.Properties.ListProperties;

public sealed class ListPropertiesValidator : AbstractValidator<ListPropertiesQuery>
{
    public ListPropertiesValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than zero.");

        RuleFor(request => request.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than zero.");
    }
}
