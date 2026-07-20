using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using RealtorApi.Infrastructure.Api;
using RealtorApi.Infrastructure.Validation;

namespace RealtorApiTests.Infrastructure.TestSlices
{
    public class ValidationTestSlice : ISlice
    {
        public void Register(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/validation", (ValidationRequest request) => Results.Ok(request))
                .AddValidation<ValidationRequest>();

            endpoints.MapPost("/validation-no-validator", (NoValidatorRequest request) => Results.Ok(request))
                .AddValidation<NoValidatorRequest>();
        }
    }

    public sealed record ValidationRequest(string Name, int Age);

    public sealed record NoValidatorRequest(string Data);
}
