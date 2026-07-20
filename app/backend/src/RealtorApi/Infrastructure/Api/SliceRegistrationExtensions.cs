using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RealtorApi.Infrastructure.Api
{
    public static class SliceRegistrationExtensions
    {
        public static IServiceCollection AddSlices(this IServiceCollection services, params Assembly[] assemblies)
        {
            var candidates = assemblies?.Any() == true
                ? assemblies
                : AppDomain.CurrentDomain.GetAssemblies();

            var sliceTypes = candidates
                .Where(a => a?.IsDynamic == false)
                .SelectMany(a => a.GetTypes())
                .Where(type => typeof(ISlice).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .Distinct();

            foreach (var sliceType in sliceTypes)
            {
                services.AddTransient(typeof(ISlice), sliceType);
            }

            return services;
        }

        public static IServiceCollection AddSlices(this IServiceCollection services)
            => services.AddSlices(AppDomain.CurrentDomain.GetAssemblies());
    }
}
