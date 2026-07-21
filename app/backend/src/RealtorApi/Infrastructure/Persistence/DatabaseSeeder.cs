using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using RealtorApi.Domain.Properties;

namespace RealtorApi.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public DatabaseSeeder(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var contentRoot = _env.ContentRootPath;
        var seedDir = Path.Combine(contentRoot, "support", "seed-data");

        if (!Directory.Exists(seedDir)) return;

        var manifestPath = Path.Combine(seedDir, "seed-manifest.json");
        if (!File.Exists(manifestPath)) return;

        var manifestJson = await File.ReadAllTextAsync(manifestPath, cancellationToken);
        var manifest = JsonSerializer.Deserialize<SeedManifest>(manifestJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (manifest == null) return;

        var propertiesPath = Path.Combine(seedDir, manifest.PropertiesJson ?? "properties.json");
        if (!File.Exists(propertiesPath)) return;

        var propertiesJson = await File.ReadAllTextAsync(propertiesPath, cancellationToken);
        var rawProperties = JsonSerializer.Deserialize<List<SeedProperty>>(propertiesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<SeedProperty>();

        foreach (var sp in rawProperties)
        {
            // Idempotency: check by Title (assumed unique for seed)
            var existing = await _db.Properties.FirstOrDefaultAsync(p => p.Title == sp.Title, cancellationToken);
            if (existing != null)
            {
                existing.Description = sp.Description ?? existing.Description;
                existing.Address = sp.Address ?? existing.Address;
                existing.Price = sp.Price ?? existing.Price;
                existing.BedroomCount = sp.BedroomCount ?? existing.BedroomCount;
                existing.BathroomCount = sp.BathroomCount ?? existing.BathroomCount;
                existing.AreaSquareMeters = sp.AreaSquareMeters ?? existing.AreaSquareMeters;
                existing.UpdatedAt = DateTime.UtcNow;
                if (!string.IsNullOrWhiteSpace(sp.ImageFileName) && !string.IsNullOrWhiteSpace(manifest.ImageUrlBase))
                {
                    existing.ImageUrl = PathCombineForUrl(manifest.ImageUrlBase, sp.ImageFileName);
                }
            }
            else
            {
                var prop = new Property
                {
                    Id = sp.Id != Guid.Empty ? sp.Id : Guid.NewGuid(),
                    Title = sp.Title ?? "",
                    Description = sp.Description ?? "",
                    Address = sp.Address ?? "",
                    Price = sp.Price ?? 0m,
                    BedroomCount = sp.BedroomCount ?? 0,
                    BathroomCount = sp.BathroomCount ?? 0,
                    AreaSquareMeters = sp.AreaSquareMeters ?? 0m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = Enum.TryParse<PropertyStatus>(sp.Status, true, out var s) ? s : PropertyStatus.Available,
                    ImageUrl = !string.IsNullOrWhiteSpace(sp.ImageFileName) && !string.IsNullOrWhiteSpace(manifest.ImageUrlBase)
                        ? PathCombineForUrl(manifest.ImageUrlBase, sp.ImageFileName)
                        : string.Empty
                };

                await _db.Properties.AddAsync(prop, cancellationToken);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        // Copy/sync images referenced in manifest
        if (!string.IsNullOrWhiteSpace(manifest.ImagesSourceDirectory) && !string.IsNullOrWhiteSpace(manifest.ImagesPublicDirectory))
        {
            var sourceDir = Path.Combine(seedDir, manifest.ImagesSourceDirectory);
            var destDir = Path.Combine(_env.ContentRootPath, manifest.ImagesPublicDirectory.TrimStart('/', '\\'));
            if (Directory.Exists(sourceDir))
            {
                Directory.CreateDirectory(destDir);
                foreach (var file in Directory.GetFiles(sourceDir))
                {
                    var destFile = Path.Combine(destDir, Path.GetFileName(file));
                    File.Copy(file, destFile, true);
                }
            }
        }
    }

    private static string PathCombineForUrl(string baseUrl, string fileName)
    {
        return baseUrl.TrimEnd('/') + "/" + fileName.TrimStart('/');
    }

    private record SeedManifest
    (
        string? PropertiesJson,
        string? PropertyStatusesJson,
        string? ImagesSourceDirectory,
        string? ImagesPublicDirectory,
        string? ImageUrlBase
    );

    private record SeedProperty
    (
        Guid Id,
        string Title,
        string? Description,
        string? Address,
        decimal? Price,
        string? Status,
        int? BedroomCount,
        int? BathroomCount,
        decimal? AreaSquareMeters,
        string? ImageFileName
    );
}
