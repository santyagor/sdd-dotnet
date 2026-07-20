using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RealtorApi.Infrastructure.Handlers
{
    public static class HandlerRegistrationExtensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            var candidates = assemblies?.Any() == true
                ? assemblies
                : AppDomain.CurrentDomain.GetAssemblies();

            var handlerTypes = candidates
                .Where(a => a?.IsDynamic == false)
                .SelectMany(a => a.GetTypes())
                .Where(type => typeof(IHandler).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .Distinct();

            foreach (var handlerType in handlerTypes)
            {
                services.AddTransient(handlerType);
                services.AddTransient(typeof(IHandler), handlerType);
            }

            return services;
        }
    }
}
