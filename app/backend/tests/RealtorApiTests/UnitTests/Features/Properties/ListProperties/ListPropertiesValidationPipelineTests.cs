using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RealtorApi.Features.Properties.ListProperties;
using RealtorApi.Infrastructure.Validation;

namespace RealtorApiTests.UnitTests.Features.Properties.ListProperties;

public class ListPropertiesValidationPipelineTests
{
    [Fact]
    public async Task InvalidQuery_ProducesValidationProblemDetails()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ListPropertiesValidator>();
        services.AddTransient<FluentValidation.IValidator<ListPropertiesQuery>>(sp => sp.GetRequiredService<ListPropertiesValidator>());
        services.AddSingleton<ValidationFilterFactory>();

        await using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ValidationFilterFactory>();

        var validationProblemDetails = await factory.ValidateAsync(new ListPropertiesQuery(0, 6), CancellationToken.None);

        validationProblemDetails.Should().NotBeNull();
        validationProblemDetails!.Status.Should().Be(StatusCodes.Status400BadRequest);
        validationProblemDetails.Title.Should().Be("Validation failed");
        validationProblemDetails.Errors.Should().ContainKey("Page");
    }

    [Fact]
    public async Task ValidQuery_PassesThroughWithoutError()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ListPropertiesValidator>();
        services.AddTransient<FluentValidation.IValidator<ListPropertiesQuery>>(sp => sp.GetRequiredService<ListPropertiesValidator>());
        services.AddSingleton<ValidationFilterFactory>();

        await using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ValidationFilterFactory>();

        var validationProblemDetails = await factory.ValidateAsync(new ListPropertiesQuery(1, 6), CancellationToken.None);

        validationProblemDetails.Should().BeNull();
    }
}
