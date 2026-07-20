using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using RealtorApi.Infrastructure.Api;
using RealtorApi.Infrastructure.Results;

namespace RealtorApiTests.Infrastructure.TestSlices
{
    public class ErrorResultTestSlice : ISlice
    {
        public void Register(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/error-result", (ResultProblemDetailsMapper mapper) =>
                mapper.MapResult(Result.Failure("expected_error", "Expected error occurred", 422, "The test error was produced.")));
        }
    }
}
