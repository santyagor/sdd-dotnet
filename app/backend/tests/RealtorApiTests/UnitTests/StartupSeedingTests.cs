using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.UnitTests;

public class StartupSeedingTests
{
    [Fact]
    public async Task StartupExecutesMigrationAndSeeding()
    {
        // Use a temp directory to isolate from actual project seed data
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "app", "backend", "src", "RealtorApi");
        Directory.CreateDirectory(tempDir);

        try
        {
            var seedDir = Path.Combine(tempDir, "support", "seed-data");
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
                "\"id\": \"d7db357b-ec73-4ad4-b574-25a95f577dc6\"," +
                "\"title\": \"Casa de startup test\"," +
                "\"description\": \"Descripción de startup test\"," +
                "\"address\": \"456 Startup Ave\"," +
                "\"price\": 2500.50," +
                "\"status\": \"Available\"," +
                "\"bedroomCount\": 3," +
                "\"bathroomCount\": 2," +
                "\"areaSquareMeters\": 185.75," +
                "\"imageUrl\": \"startup.png\"," +
                "\"createdAt\": \"2026-07-20T12:00:00Z\"," +
                "\"updatedAt\": null" +
                "}]";

            File.WriteAllText(Path.Combine(seedDir, "properties.json"), propertyJson);
            File.WriteAllText(Path.Combine(imageSourceDir, "startup.png"), "startup image");

            // Test using InMemoryDatabase (migrations not needed/supported)
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddRouting();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            builder.Services.AddScoped<DatabaseSeeder>();

            // Mock the environment to use our temp directory
            builder.Services.AddScoped<IWebHostEnvironment>(sp => 
                new TestWebHostEnvironment(tempDir));

            await using var app = builder.Build();

            // Manually ensure database creation and run seeding
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();

            await db.Database.EnsureCreatedAsync();
            await seeder.SeedAsync();

            var properties = await db.Properties.ToListAsync();
            properties.Should().HaveCount(1);
            properties[0].Title.Should().Be("Casa de startup test");
            properties[0].ImageUrl.Should().Be("/images/properties/startup.png");
        }
        finally
        {
            Directory.Delete(Path.Combine(Path.GetTempPath(), Path.GetTempPath().Split(Path.DirectorySeparatorChar).Last()), true);
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
            ContentRootFileProvider = new PhysicalFileProvider(contentRootPath);
            WebRootFileProvider = new PhysicalFileProvider(WebRootPath);
        }

        public string ApplicationName { get; set; }
        public string EnvironmentName { get; set; }
        public string ContentRootPath { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
    }
}
