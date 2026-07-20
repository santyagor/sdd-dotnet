using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RealtorApi.Infrastructure.Validation
{
    public class ValidationFilterFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilterFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<ValidationProblemDetails?> ValidateAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        {
            var validator = _serviceProvider.GetService<IValidator<TRequest>>();
            if (validator is null)
            {
                return null;
            }

            ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult.IsValid)
            {
                return null;
            }

            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());

            return new ValidationProblemDetails(errors)
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };
        }
    }
}
