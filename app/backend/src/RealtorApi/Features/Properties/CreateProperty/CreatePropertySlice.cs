using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RealtorApi.Infrastructure.Api;
using RealtorApi.Infrastructure.Validation;

namespace RealtorApi.Features.Properties.CreateProperty;

public sealed class CreatePropertySlice : ISlice
{
    public void Register(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/properties", HandleAsync)
            .AddValidation<CreatePropertyRequest>()
            .WithName("CreateProperty");
    }

    private static async Task<IResult> HandleAsync(
        [FromForm] CreatePropertyRequest request,
        [FromServices] CreatePropertyHandler handler,
        CancellationToken cancellationToken)
    {
        var property = await handler.HandleAsync(request, cancellationToken);
        var response = property.ToResponse();
        return Results.Created($"/api/properties/{property.Id}", response);
    }
}