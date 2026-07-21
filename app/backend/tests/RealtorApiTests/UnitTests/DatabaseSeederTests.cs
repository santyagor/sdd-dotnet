using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.UnitTests;

public class DatabaseSeederTests
{
    [Fact]
    public async Task SeedAsync_InsertsPropertiesFromSeedDataAndCopiesImagesToPublicDirectory()
    {
        var rootDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(rootDir);

        try
        {
            var seedDir = Path.Combine(rootDir, "support", "seed-data");
            var imageSourceDir = Path.Combine(seedDir, "properties");
            Directory.CreateDirectory(imageSourceDir);

            var manifest = "{" +
                "\"propertiesJson\": \"properties.json\"," +
                "\"propertyStatusesJson\": \"properties-statuses.json\"," +
                "\"imagesSourceDirectory\": \"properties\"," +
                "\"imagesPublicDirectory\": \"wwwroot/images/properties\"," +
                "\"imageUrlBase\": \"/images/properties\"" +
                "}";

            File.WriteAllText(Path.Combine(seedDir, "seed-manifest.json"), manifest);
            File.WriteAllText(Path.Combine(seedDir, "properties-statuses.json"), "[\"Available\",\"Rented\",\"Maintenance\"]");

            var propertyJson = "[{" +
                "\"id\": \"d7db357b-ec73-4ad4-b574-25a95f577dc5\"," +
                "\"title\": \"Casa de prueba\"," +
                "\"description\": \"Descripción de prueba\"," +
                "\"address\": \"123 Test St\"," +
                "\"price\": 1000.00," +
                "\"status\": \"Available\"," +
                "\"bedroomCount\": 2," +
                "\"bathroomCount\": 1," +
                "\"areaSquareMeters\": 120.5," +
                "\"imageUrl\": \"house1.png\"," +
                "\"createdAt\": \"2026-07-20T12:00:00Z\"," +
                "\"updatedAt\": \"2026-07-20T12:30:00Z\"" +
                "}]";

            File.WriteAllText(Path.Combine(seedDir, "properties.json"), propertyJson);
            File.WriteAllText(Path.Combine(imageSourceDir, "house1.png"), "test image");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new AppDbContext(options);
            await db.Database.EnsureCreatedAsync();

            var seeder = new DatabaseSeeder(db, new TestWebHostEnvironment(rootDir));

            await seeder.SeedAsync();
            var property = await db.Properties.SingleAsync();

            property.Title.Should().Be("Casa de prueba");
            property.Status.Should().Be(RealtorApi.Domain.Properties.PropertyStatus.Available);
            property.ImageUrl.Should().Be("/images/properties/house1.png");
            File.Exists(Path.Combine(rootDir, "wwwroot", "images", "properties", "house1.png")).Should().BeTrue();

            await seeder.SeedAsync();
            (await db.Properties.CountAsync()).Should().Be(1);
        }
        finally
        {
            Directory.Delete(rootDir, true);
        }
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
