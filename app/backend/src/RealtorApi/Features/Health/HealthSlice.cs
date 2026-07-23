using Microsoft.AspNetCore.OpenApi;
using RealtorApi.Infrastructure.Api;

namespace RealtorApi.Features.Health
{
    public class HealthSlice : ISlice
    {
        public void Register(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/health", () => Results.Ok(new HealthResponse("Healthy")))
                .WithName("Health")
                .WithSummary("Get service health")
                .WithDescription("Returns the current health status of RealtorApi.")
                .Produces<HealthResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }

    public sealed record HealthResponse(string Status);
}
