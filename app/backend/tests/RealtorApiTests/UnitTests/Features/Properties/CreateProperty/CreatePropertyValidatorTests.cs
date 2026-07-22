using System.Text;
using Microsoft.AspNetCore.Http;
using RealtorApi.Features.Properties.CreateProperty;

namespace RealtorApiTests.UnitTests.Features.Properties.CreateProperty;

public class CreatePropertyValidatorTests
{
    private readonly CreatePropertyValidator _validator = new();

    [Fact]
    public async Task Validate_WhenRequestIsCompleteAndImageIsValid_Passes()
    {
        var request = CreateValidRequest(CreateImageFile("front-yard.png", "image/png", ValidPngBytes()));

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WhenImageHasInvalidSignature_Fails()
    {
        var request = CreateValidRequest(CreateImageFile("front-yard.jpg", "image/jpeg", Encoding.UTF8.GetBytes("not-a-real-image")));

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreatePropertyRequest.ImageFile));
    }

    [Fact]
    public async Task Validate_WhenImageExceedsFiveMegabytes_Fails()
    {
        var oversizedBytes = new byte[5 * 1024 * 1024 + 1];
        Array.Copy(ValidPngBytes(), oversizedBytes, ValidPngBytes().Length);

        var request = CreateValidRequest(CreateImageFile("large.png", "image/png", oversizedBytes));

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreatePropertyRequest.ImageFile));
    }

    [Fact]
    public async Task Validate_WhenRequiredFieldsAreMissing_Fails()
    {
        var request = new CreatePropertyRequest();

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreatePropertyRequest.Title));
        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreatePropertyRequest.Price));
        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreatePropertyRequest.Status));
    }

    private static CreatePropertyRequest CreateValidRequest(IFormFile? imageFile = null)
    {
        return new CreatePropertyRequest
        {
            Title = "Casa válida",
            Description = "Descripción válida",
            Address = "123 Example St",
            Price = 2750m,
            Status = PropertyStatus.Available,
            BedroomCount = 3,
            BathroomCount = 2,
            AreaSquareMeters = 140.25m,
            ImageFile = imageFile
        };
    }

    private static IFormFile CreateImageFile(string fileName, string contentType, byte[] bytes)
    {
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "imageFile", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private static byte[] ValidPngBytes()
    {
        return new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52
        };
    }
}