using RealtorApi.Infrastructure.Api;

namespace RealtorApi.Features.Health
{
    public class HealthSlice : ISlice
    {
        public void Register(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
                .WithName("Health");
        }
    }
}
