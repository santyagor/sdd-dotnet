using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace RealtorApi.Infrastructure.Validation
{
    public static class ValidationEndpointExtensions
    {
        public static RouteHandlerBuilder AddValidation<TRequest>(this RouteHandlerBuilder builder)
        {
            builder.AddEndpointFilter(async (context, next) =>
            {
                var request = context.GetArgument<TRequest>(0);
                var validationFactory = context.HttpContext.RequestServices.GetRequiredService<ValidationFilterFactory>();
                var validationProblemDetails = await validationFactory.ValidateAsync(request, context.HttpContext.RequestAborted);
                if (validationProblemDetails is not null)
                {
                    return Microsoft.AspNetCore.Http.Results.ValidationProblem(
                        validationProblemDetails.Errors,
                        title: validationProblemDetails.Title,
                        type: validationProblemDetails.Type,
                        statusCode: StatusCodes.Status400BadRequest);
                }

                return await next(context);
            });

            return builder;
        }
    }
}
