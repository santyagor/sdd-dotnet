using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RealtorApi.Infrastructure.Api;
using RealtorApi.Infrastructure.Results;
using RealtorApi.Infrastructure.Validation;

namespace RealtorApi.Features.Properties.UpdateProperty;

public sealed class UpdatePropertySlice : ISlice
{
    public void Register(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/properties/{id:guid}", HandleAsync)
            .AddValidation<UpdatePropertyRequest>()
            .WithName("UpdateProperty");
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromForm] UpdatePropertyRequest request,
        [FromServices] UpdatePropertyHandler handler,
        [FromServices] ResultProblemDetailsMapper mapper,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, request, cancellationToken);
        return mapper.MapResult(result);
    }
}
