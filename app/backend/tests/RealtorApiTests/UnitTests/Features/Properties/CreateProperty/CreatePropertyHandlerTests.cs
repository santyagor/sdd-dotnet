using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using RealtorApi.Features.Properties.CreateProperty;

namespace RealtorApiTests.UnitTests.Features.Properties.CreateProperty;

public class CreatePropertyHandlerTests
{
    [Fact]
    public async Task HandleAsync_CreatesPropertyWithoutImageAndPersistsNullImageUrl()
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

            var request = CreateRequest();

            var property = await handler.HandleAsync(request, CancellationToken.None);

            property.ImageUrl.Should().BeNull();
            property.Title.Should().Be(request.Title);
            (await dbContext.Properties.CountAsync()).Should().Be(1);

            var persisted = await dbContext.Properties.SingleAsync();
            persisted.ImageUrl.Should().BeNull();
            persisted.Title.Should().Be(request.Title);
        }
        finally
        {
            Directory.Delete(rootDir, true);
        }
    }

    private static CreatePropertyRequest CreateRequest()
    {
        return new CreatePropertyRequest
        {
            Title = "Casa sin imagen",
            Description = "Descripción de prueba",
            Address = "123 Main St",
            Price = 2500m,
            Status = PropertyStatus.Available,
            BedroomCount = 3,
            BathroomCount = 2,
            AreaSquareMeters = 145.5m
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