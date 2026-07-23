using FluentValidation;

namespace RealtorApi.Features.Properties.UpdatePropertyStatus;

public sealed class UpdatePropertyStatusValidator : AbstractValidator<UpdatePropertyStatusRequest>
{
    public UpdatePropertyStatusValidator()
    {
        RuleFor(request => request.Status)
            .NotNull()
            .WithMessage("Status is required.")
            .IsInEnum()
            .WithMessage("Status must be a valid property status.");
    }
}
