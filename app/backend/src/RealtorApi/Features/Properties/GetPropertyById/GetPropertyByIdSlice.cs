using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using RealtorApi.Infrastructure.Api;
using RealtorApi.Infrastructure.Results;
using RealtorApi.Infrastructure.Validation;

namespace RealtorApi.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdSlice : ISlice
{
    public void Register(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/properties/{id}", HandleAsync)
            .AddValidation<GetPropertyByIdRequest>()
            .WithName("GetPropertyById")
            .WithSummary("Get property by id")
            .WithDescription("Returns the public details for a single property.")
            .Produces<PropertyDetailResponse>(StatusCodes.Status200OK)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetPropertyByIdRequest request,
        HttpContext httpContext,
        [FromServices] GetPropertyByIdHandler handler,
        [FromServices] ResultProblemDetailsMapper mapper,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, httpContext, cancellationToken);
        return mapper.MapResult(result);
    }
}
