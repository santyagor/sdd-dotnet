using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.UnitTests;

public class PropertyImageHostingTests
{
    [Fact]
    public async Task ImagesAreCopiedToPublicDirectoryAndAccessibleFromFinalLocation()
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

            // Create multiple properties with different images
            var propertyJson = "[" +
                "{\"id\": \"d7db357b-ec73-4ad4-b574-25a95f577d01\"," +
                "\"title\": \"Propiedad 1\"," +
                "\"description\": \"Desc 1\"," +
                "\"address\": \"Addr 1\"," +
                "\"price\": 1000.00," +
                "\"status\": \"Available\"," +
                "\"bedroomCount\": 2," +
                "\"bathroomCount\": 1," +
                "\"areaSquareMeters\": 100.00," +
                "\"imageUrl\": \"img1.png\"," +
                "\"createdAt\": \"2026-07-20T12:00:00Z\"," +
                "\"updatedAt\": null}," +
                "{\"id\": \"d7db357b-ec73-4ad4-b574-25a95f577d02\"," +
                "\"title\": \"Propiedad 2\"," +
                "\"description\": \"Desc 2\"," +
                "\"address\": \"Addr 2\"," +
                "\"price\": 2000.00," +
                "\"status\": \"Rented\"," +
                "\"bedroomCount\": 3," +
                "\"bathroomCount\": 2," +
                "\"areaSquareMeters\": 150.00," +
                "\"imageUrl\": \"img2.png\"," +
                "\"createdAt\": \"2026-07-20T12:00:00Z\"," +
                "\"updatedAt\": null}" +
                "]";

            File.WriteAllText(Path.Combine(seedDir, "properties.json"), propertyJson);
            File.WriteAllText(Path.Combine(imageSourceDir, "img1.png"), "image 1 content");
            File.WriteAllText(Path.Combine(imageSourceDir, "img2.png"), "image 2 content");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new AppDbContext(options);
            await db.Database.EnsureCreatedAsync();

            var seeder = new DatabaseSeeder(db, new TestWebHostEnvironment(rootDir));
            await seeder.SeedAsync();

            // Verify both properties images are at public location
            var publicDir = Path.Combine(rootDir, "wwwroot", "images", "properties");
            Directory.Exists(publicDir).Should().BeTrue();

            var img1Path = Path.Combine(publicDir, "img1.png");
            var img2Path = Path.Combine(publicDir, "img2.png");

            File.Exists(img1Path).Should().BeTrue();
            File.Exists(img2Path).Should().BeTrue();

            // Verify image content matches
            File.ReadAllText(img1Path).Should().Be("image 1 content");
            File.ReadAllText(img2Path).Should().Be("image 2 content");

            // Verify properties have correct ImageUrl pointing to public location
            var properties = await db.Properties.OrderBy(p => p.Title).ToListAsync();
            properties.Should().HaveCount(2);

            properties[0].ImageUrl.Should().Be("/images/properties/img1.png");
            properties[1].ImageUrl.Should().Be("/images/properties/img2.png");
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
