using Microsoft.AspNetCore.Routing;

namespace RealtorApi.Infrastructure.Api
{
    public interface ISlice
    {
        void Register(IEndpointRouteBuilder endpoints);
    }
}
