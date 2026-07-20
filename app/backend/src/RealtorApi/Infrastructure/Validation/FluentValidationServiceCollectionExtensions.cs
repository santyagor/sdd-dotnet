using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RealtorApi.Infrastructure.Validation
{
    public static class FluentValidationServiceCollectionExtensions
    {
        public static IServiceCollection AddFluentValidation(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddFluentValidationValidators(assemblies);
            services.AddSingleton<ValidationFilterFactory>();
            return services;
        }
    }
}
