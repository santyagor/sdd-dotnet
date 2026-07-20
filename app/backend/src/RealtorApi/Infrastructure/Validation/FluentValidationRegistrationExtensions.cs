using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace RealtorApi.Infrastructure.Validation
{
    public static class FluentValidationRegistrationExtensions
    {
        public static IServiceCollection AddFluentValidationValidators(this IServiceCollection services, params Assembly[] assemblies)
        {
            var candidates = assemblies?.Any() == true
                ? assemblies
                : AppDomain.CurrentDomain.GetAssemblies();

            var validatorEntries = candidates
                .Where(a => a?.IsDynamic == false)
                .SelectMany(a => a.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition)
                .SelectMany(type => type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))
                    .Select(i => new { ValidatorType = type, InterfaceType = i }))
                .Distinct();

            foreach (var entry in validatorEntries)
            {
                services.AddTransient(entry.InterfaceType, entry.ValidatorType);
            }

            return services;
        }
    }
}
