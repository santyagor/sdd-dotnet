using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RealtorApi.Features.Properties.CreateProperty;
using RealtorApi.Infrastructure.Validation;

namespace RealtorApiTests.UnitTests.Features.Properties.CreateProperty;

public class CreatePropertyValidationPipelineTests
{
    [Fact]
    public async Task InvalidRequest_ProducesValidationProblemDetails()
    {
        var services = new ServiceCollection();
        services.AddSingleton<CreatePropertyValidator>();
        services.AddTransient<IValidator<CreatePropertyRequest>>(sp => sp.GetRequiredService<CreatePropertyValidator>());
        services.AddSingleton<ValidationFilterFactory>();

        await using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ValidationFilterFactory>();

        var request = new CreatePropertyRequest
        {
            Title = "",
            Description = "Descripción",
            Address = "Dirección",
            Price = 2000m,
            Status = PropertyStatus.Available,
            BedroomCount = 2,
            BathroomCount = 1,
            AreaSquareMeters = 100m,
            ImageFile = CreateInvalidImageFile()
        };

        var validationProblemDetails = await factory.ValidateAsync(request, CancellationToken.None);

        validationProblemDetails.Should().NotBeNull();
        validationProblemDetails!.Status.Should().Be(StatusCodes.Status400BadRequest);
        validationProblemDetails.Title.Should().Be("Validation failed");
        validationProblemDetails.Errors.Should().ContainKey(nameof(CreatePropertyRequest.Title));
        validationProblemDetails.Errors.Should().ContainKey(nameof(CreatePropertyRequest.ImageFile));
    }

    [Fact]
    public async Task ValidRequest_PassesThroughWithoutError()
    {
        var services = new ServiceCollection();
        services.AddSingleton<CreatePropertyValidator>();
        services.AddTransient<IValidator<CreatePropertyRequest>>(sp => sp.GetRequiredService<CreatePropertyValidator>());
        services.AddSingleton<ValidationFilterFactory>();

        await using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ValidationFilterFactory>();

        var request = new CreatePropertyRequest
        {
            Title = "Casa válida",
            Description = "Descripción válida",
            Address = "Dirección válida",
            Price = 2500m,
            Status = PropertyStatus.Available,
            BedroomCount = 3,
            BathroomCount = 2,
            AreaSquareMeters = 145m,
            ImageFile = CreateValidImageFile()
        };

        var validationProblemDetails = await factory.ValidateAsync(request, CancellationToken.None);

        validationProblemDetails.Should().BeNull();
    }

    private static IFormFile CreateInvalidImageFile()
    {
        var bytes = Encoding.UTF8.GetBytes("plain text masquerading as image");
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, bytes.Length, "imageFile", "bad.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }

    private static IFormFile CreateValidImageFile()
    {
        var bytes = new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52
        };
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, bytes.Length, "imageFile", "good.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };
    }
}