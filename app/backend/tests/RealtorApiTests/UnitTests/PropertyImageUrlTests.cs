using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.UnitTests;

public class PropertyImageUrlTests
{
    [Fact]
    public async Task PropertyImageUrlIsPublicPathAfterSeeding()
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
                "\"imagesPublicDirectory\": \"wwwroot/assets/properties\"," +
                "\"imageUrlBase\": \"/assets/properties\"" +
                "}";

            File.WriteAllText(Path.Combine(seedDir, "seed-manifest.json"), manifest);
            File.WriteAllText(Path.Combine(seedDir, "properties-statuses.json"), "[\"Available\",\"Rented\",\"Maintenance\"]");

            var propertyJson = "[{" +
                "\"id\": \"d7db357b-ec73-4ad4-b574-25a95f577dc7\"," +
                "\"title\": \"Casa con imagen URL pública\"," +
                "\"description\": \"Prueba de ImageUrl\"," +
                "\"address\": \"789 Image Blvd\"," +
                "\"price\": 3200.00," +
                "\"status\": \"Available\"," +
                "\"bedroomCount\": 3," +
                "\"bathroomCount\": 2," +
                "\"areaSquareMeters\": 195.50," +
                "\"imageUrl\": \"property-test.png\"," +
                "\"createdAt\": \"2026-07-20T12:00:00Z\"," +
                "\"updatedAt\": null" +
                "}]";

            File.WriteAllText(Path.Combine(seedDir, "properties.json"), propertyJson);
            File.WriteAllText(Path.Combine(imageSourceDir, "property-test.png"), "test image data");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new AppDbContext(options);
            await db.Database.EnsureCreatedAsync();

            var seeder = new DatabaseSeeder(db, new TestWebHostEnvironment(rootDir));
            await seeder.SeedAsync();

            var property = await db.Properties.FirstAsync();

            // Verify ImageUrl is a public path (starts with /), not a file system path
            property.ImageUrl.Should().NotBeNullOrEmpty();
            property.ImageUrl.Should().StartWith("/assets/properties");
            property.ImageUrl.Should().Contain("property-test.png");
            property.ImageUrl.Should().NotContain("\\");
            property.ImageUrl.Should().NotContain("wwwroot");
            property.ImageUrl.Should().NotContain("support");

            // Verify image file was copied to public directory
            var publicImagePath = Path.Combine(rootDir, "wwwroot", "assets", "properties", "property-test.png");
            File.Exists(publicImagePath).Should().BeTrue();
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
