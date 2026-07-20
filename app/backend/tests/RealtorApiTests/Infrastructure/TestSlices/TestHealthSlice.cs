using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using RealtorApi.Infrastructure.Api;

namespace RealtorApiTests.Infrastructure.TestSlices
{
    public class TestHealthSlice : ISlice
    {
        public void Register(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/test-health", () => Results.Ok(new { status = "Healthy", source = "test-slice" }));
        }
    }
}
