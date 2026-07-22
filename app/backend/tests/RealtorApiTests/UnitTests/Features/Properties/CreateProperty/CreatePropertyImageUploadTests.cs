using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using RealtorApi.Features.Properties.CreateProperty;

namespace RealtorApiTests.UnitTests.Features.Properties.CreateProperty;

public class CreatePropertyImageUploadTests
{
    [Fact]
    public async Task HandleAsync_StoresImageWithUniqueFileNameAndOriginalExtension()
    {
        var rootDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(rootDir);

        try
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            await using var dbContext = new AppDbContext(options);
            var handler = new CreatePropertyHandler(
                dbContext,
                new TestWebHostEnvironment(rootDir),
                NullLogger<CreatePropertyHandler>.Instance);

            var request = CreateRequest(CreatePngFile("front-yard.png"));

            var property = await handler.HandleAsync(request, CancellationToken.None);

            property.ImageUrl.Should().StartWith("/assets/properties/");
            property.ImageUrl.Should().EndWith(".png");
            property.ImageUrl.Should().NotContain("front-yard.png");

            var storedFileName = Path.GetFileName(property.ImageUrl);
            storedFileName.Should().NotBeNullOrWhiteSpace();

            var storedFilePath = Path.Combine(rootDir, "wwwroot", "assets", "properties", storedFileName);
            File.Exists(storedFilePath).Should().BeTrue();
        }
        finally
        {
            Directory.Delete(rootDir, true);
        }
    }

    private static CreatePropertyRequest CreateRequest(IFormFile imageFile)
    {
        return new CreatePropertyRequest
        {
            Title = "Casa con imagen",
            Description = "Descripción de prueba",
            Address = "123 Ocean Drive",
            Price = 3200m,
            Status = PropertyStatus.Available,
            BedroomCount = 4,
            BathroomCount = 3,
            AreaSquareMeters = 210.75m,
            ImageFile = imageFile
        };
    }

    private static IFormFile CreatePngFile(string fileName)
    {
        var bytes = new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52
        };

        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "imageFile", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public TestWebHostEnvironment(string contentRootPath)
        {
            ContentRootPath = contentRootPath;
            WebRootPath = Path.Combine(contentRootPath, "wwwroot");
            EnvironmentName = "Test";
            ApplicationName = "RealtorApi.Tests";
            ContentRootFileProvider = new NullFileProvider();
            WebRootFileProvider = new NullFileProvider();
        }

        public string ApplicationName { get; set; }
        public string EnvironmentName { get; set; }
        public string ContentRootPath { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
    }
}