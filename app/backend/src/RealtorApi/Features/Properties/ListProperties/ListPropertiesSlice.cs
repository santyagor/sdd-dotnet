using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RealtorApi.Infrastructure.Api;
using RealtorApi.Infrastructure.Validation;

namespace RealtorApi.Features.Properties.ListProperties;

public sealed class ListPropertiesSlice : ISlice
{
    public void Register(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/properties", HandleAsync)
            .AddValidation<ListPropertiesQuery>()
            .WithName("ListProperties");
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] ListPropertiesQuery request,
        HttpContext httpContext,
        [FromServices] ListPropertiesHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(request, httpContext, cancellationToken);
        return Results.Ok(response);
    }
}
