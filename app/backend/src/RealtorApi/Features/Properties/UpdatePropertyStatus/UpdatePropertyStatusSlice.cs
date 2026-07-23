using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using RealtorApi.Infrastructure.Api;
using RealtorApi.Infrastructure.Results;
using RealtorApi.Infrastructure.Validation;

namespace RealtorApi.Features.Properties.UpdatePropertyStatus;

public sealed class UpdatePropertyStatusSlice : ISlice
{
    public void Register(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPatch("/api/properties/{id}/status", HandleAsync)
            .AddValidation<UpdatePropertyStatusRequest>()
            .WithName("UpdatePropertyStatus")
            .WithSummary("Update property status")
            .WithDescription("Updates only the status of an existing property.")
            .Accepts<UpdatePropertyStatusRequest>("application/json")
            .Produces<UpdatePropertyStatusResponse>(StatusCodes.Status200OK)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        UpdatePropertyStatusRequest request,
        [FromServices] UpdatePropertyStatusHandler handler,
        [FromServices] ResultProblemDetailsMapper mapper,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, request, cancellationToken);
        return mapper.MapResult(result);
    }
}
