using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace RealtorApi.Infrastructure.Api
{
    public static class SliceMappingExtensions
    {
        public static IEndpointRouteBuilder MapSlices(this IEndpointRouteBuilder app)
        {
            var slices = app.ServiceProvider.GetServices<ISlice>();
            foreach (var slice in slices)
            {
                slice.Register(app);
            }

            return app;
        }
    }
}
