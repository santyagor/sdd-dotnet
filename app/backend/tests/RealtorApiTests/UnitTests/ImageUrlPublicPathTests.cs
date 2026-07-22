using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.UnitTests;

public class ImageUrlPublicPathTests
{
    [Fact]
    public async Task ImageUrlDoesNotContainFileSystemPath()
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
                "\"id\": \"d7db357b-ec73-4ad4-b574-25a95f577dc8\"," +
                "\"title\": \"Casa con URL segura\"," +
                "\"description\": \"Sin rutas de archivo\"," +
                "\"address\": \"999 Secure Path\"," +
                "\"price\": 4100.00," +
                "\"status\": \"Rented\"," +
                "\"bedroomCount\": 4," +
                "\"bathroomCount\": 3," +
                "\"areaSquareMeters\": 280.00," +
                "\"imageUrl\": \"secure-image.png\"," +
                "\"createdAt\": \"2026-07-20T12:00:00Z\"," +
                "\"updatedAt\": \"2026-07-20T13:00:00Z\"" +
                "}]";

            File.WriteAllText(Path.Combine(seedDir, "properties.json"), propertyJson);
            File.WriteAllText(Path.Combine(imageSourceDir, "secure-image.png"), "secure image");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new AppDbContext(options);
            await db.Database.EnsureCreatedAsync();

            var seeder = new DatabaseSeeder(db, new TestWebHostEnvironment(rootDir));
            await seeder.SeedAsync();

            var property = await db.Properties.FirstAsync();

            // Ensure ImageUrl is NOT a file system path
            property.ImageUrl.Should().NotContain(rootDir);
            property.ImageUrl.Should().NotContain("support\\");
            property.ImageUrl.Should().NotContain("support/");
            property.ImageUrl.Should().NotContain("seed-data");
            property.ImageUrl.Should().NotContain("properties\\");  // relative to seed-data
            property.ImageUrl.Should().NotContain("wwwroot\\");
            property.ImageUrl.Should().NotContain("wwwroot/");

            // Ensure ImageUrl is an HTTP-consumable path
            property.ImageUrl.Should().StartWith("/");
            property.ImageUrl.Should().Be("/assets/properties/secure-image.png");
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
